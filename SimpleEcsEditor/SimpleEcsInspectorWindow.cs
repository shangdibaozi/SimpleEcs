#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleEcs
{
    public class SimpleEcsInspectorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private Dictionary<string, bool> _aspectFoldouts = new Dictionary<string, bool>();
        private Dictionary<Entity, bool> _entityFoldouts = new Dictionary<Entity, bool>();
        private Dictionary<object, bool> _componentFoldouts = new Dictionary<object, bool>();

        private string _selectedAspectName = "";

        [MenuItem("Tools/Simple ECS Inspector")]
        public static void ShowWindow()
        {
            var window = GetWindow<SimpleEcsInspectorWindow>("SimpleECS Inspector");
            window.Show();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("SimpleECS Inspector只在运行时可用", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            try
            {
                DrawWorldAspects();
            }
            catch (Exception e)
            {
                Debug.LogError($"ECS Inspector GUI Error: {e.Message}");
            }

            EditorGUILayout.EndScrollView();

            // 刷新面板
            if (Event.current.type == EventType.Layout)
            {
                Repaint();
            }
        }

        private void DrawWorldAspects()
        {
            EditorGUILayout.LabelField("Aspects", EditorStyles.boldLabel);
            EditorGUILayout.Space();


            var aspects = BaseAspect.Aspects;

            if (aspects.Length == 0)
            {
                EditorGUILayout.HelpBox("没有找到Aspect", MessageType.Warning);
                return;
            }

            for(var i = 0; i < aspects.Length; i++)
            {
                var aspect = aspects[i];
                var aspectName = aspect.AspectName;
                _aspectFoldouts.TryAdd(aspectName, false);

                _aspectFoldouts[aspectName] = EditorGUILayout.Foldout(
                    _aspectFoldouts[aspectName],
                    $"{aspectName}",
                    true, EditorStyles.foldoutHeader);

                if (_aspectFoldouts[aspectName])
                {
                    EditorGUI.indentLevel++;
                    try
                    {
                        DrawAspectDetails(aspect);
                    }
                    catch (Exception e)
                    {
                        EditorGUILayout.LabelField($"Aspect显示错误: {e.Message}");
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawAspectDetails(BaseAspect baseAspect)
        {
            // 显示实体数量等基础信息
            EditorGUILayout.LabelField($"实体数量: {baseAspect.entityGens.Length - 1}");
            EditorGUILayout.Space();

            // 列出所有活跃的实体
            DrawEntitiesList(baseAspect);
        }

        private void DrawEntitiesList(BaseAspect baseAspect)
        {
            EditorGUILayout.LabelField("实体列表:", EditorStyles.boldLabel);

            try
            {
                // 直接访问BaseAspect的公共字段
                for (ushort i = 1; i <= baseAspect.entityGens.Length - 1; i++) // 从1开始，跳过默认实体
                {
                    try
                    {
                        // 直接从SArray的公共_data字段获取EntityMeta
                        var entityMeta = baseAspect.entityGens._data[i];

                        // 构造实体
                        var entity = new Entity
                        {
                            Version = entityMeta.version,
                            Index = i,
                            aspectId = baseAspect.aspectId
                        };
                        DrawEntityDetails(entity, baseAspect);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"处理实体 {i} 时出错: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                EditorGUILayout.LabelField($"实体列表获取失败: {e.Message}");
            }
        }

        private void DrawEntityDetails(Entity entity, BaseAspect baseAspect)
        {
            _entityFoldouts.TryAdd(entity, false);

            EditorGUILayout.BeginHorizontal();
            try
            {
                _entityFoldouts[entity] = EditorGUILayout.Foldout(
                    _entityFoldouts[entity],
                    entity.ToString(),
                    true);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }

            if (_entityFoldouts[entity])
            {
                EditorGUI.indentLevel++;
                try
                {
                    DrawEntityComponents(entity, baseAspect);
                }
                catch (Exception e)
                {
                    EditorGUILayout.LabelField($"组件显示错误: {e.Message}");
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawEntityComponents(Entity entity, BaseAspect baseAspect)
        {
            try
            {
                var pools = baseAspect.GetPools(entity);
                for (var i = 0; i < pools.Length; i++)
                {
                    DrawComponentDetails(pools[i], entity);
                }
            }
            catch (Exception e)
            {
                EditorGUILayout.LabelField($"获取组件失败: {e.Message}");
            }
        }

        private void DrawComponentDetails(ACPool pool, Entity entity)
        {
            try
            {
#if DEBUG
                var comp = pool.Get(entity);
                if (comp == null)
                {
                    return;
                }

                var componentType = comp.GetType();
                var componentKey = $"{entity.Id}_{componentType.Name}";

                _componentFoldouts.TryAdd(componentKey, false);
                var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                if (fields.Length == 0)
                {
                    EditorGUILayout.LabelField(componentType.Name);
                }
                else
                {
                    _componentFoldouts[componentKey] = EditorGUILayout.Foldout(
                        _componentFoldouts[componentKey],
                        componentType.Name,
                        true, EditorStyles.foldoutHeader);
                }

                if (_componentFoldouts[componentKey])
                {
                    EditorGUI.indentLevel++;
                    try
                    {
                        DrawComponentFields(comp, fields, entity, pool);
                    }
                    catch (Exception e)
                    {
                        EditorGUILayout.LabelField($"字段显示错误: {e.Message}");
                    }

                    EditorGUI.indentLevel--;
                }
#else
            EditorGUILayout.LabelField("组件详情仅在DEBUG模式下可用");
#endif
            }
            catch (Exception e)
            {
                EditorGUILayout.LabelField($"组件访问错误: {e.Message}");
            }
        }

        private void DrawComponentFields(object component, FieldInfo[] fields, Entity entity, ACPool pool)
        {
            var componentType = component.GetType();
            // var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var fieldKey = $"{entity.Id}_{componentType.Name}_{field.Name}";
                var fieldValue = field.GetValue(component);

                EditorGUILayout.BeginHorizontal();
                try
                {
                    EditorGUILayout.LabelField(field.Name, GUILayout.Width(120));

                    // 根据字段类型绘制不同的编辑控件
                    var newValue = DrawFieldEditor(field, fieldValue, fieldKey);

                    // 如果值发生变化，更新组件
                    if (!Equals(newValue, fieldValue))
                    {
                        try
                        {
                            field.SetValue(component, newValue);
                            pool.Set(entity, component);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"设置字段值失败: {e.Message}");
                        }
                    }

                    // 处理Unity对象的选择
                    HandleUnityObjectSelection(field, newValue);
                }
                finally
                {
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private object DrawFieldEditor(FieldInfo field, object currentValue, string fieldKey)
        {
            var fieldType = field.FieldType;

            try
            {
                // 处理基础类型
                if (fieldType == typeof(int))
                {
                    return EditorGUILayout.IntField((int)(currentValue ?? 0));
                }
                else if (fieldType == typeof(float))
                {
                    return EditorGUILayout.FloatField((float)(currentValue ?? 0f));
                }
                else if (fieldType == typeof(double))
                {
                    return EditorGUILayout.DoubleField((double)(currentValue ?? 0.0));
                }
                else if (fieldType == typeof(bool))
                {
                    return EditorGUILayout.Toggle((bool)(currentValue ?? false));
                }
                else if (fieldType == typeof(string))
                {
                    return EditorGUILayout.TextField((string)(currentValue ?? ""));
                }
                else if (fieldType == typeof(Vector2))
                {
                    return EditorGUILayout.Vector2Field("", (Vector2)(currentValue ?? Vector2.zero));
                }
                else if (fieldType == typeof(Vector3))
                {
                    return EditorGUILayout.Vector3Field("", (Vector3)(currentValue ?? Vector3.zero));
                }
                else if (fieldType == typeof(Vector4))
                {
                    return EditorGUILayout.Vector4Field("", (Vector4)(currentValue ?? Vector4.zero));
                }
                else if (fieldType == typeof(Color))
                {
                    return EditorGUILayout.ColorField((Color)(currentValue ?? Color.white));
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                {
                    // Unity对象字段
                    var unityObj = (UnityEngine.Object)currentValue;
                    var newObj = EditorGUILayout.ObjectField(unityObj, fieldType, true);
                    return newObj;
                }
                else if (fieldType.IsEnum)
                {
                    return EditorGUILayout.EnumPopup((Enum)(currentValue ?? Activator.CreateInstance(fieldType)));
                }
                else
                {
                    // 对于复杂类型，显示为只读文本
                    EditorGUILayout.LabelField(currentValue?.ToString() ?? "null");
                    return currentValue;
                }
            }
            catch (Exception e)
            {
                EditorGUILayout.LabelField($"编辑错误: {e.Message}");
                return currentValue;
            }
        }

        private void HandleUnityObjectSelection(FieldInfo field, object value)
        {
            try
            {
                // 如果字段是Transform或继承自MonoBehaviour的类型，并且被选中
                if (value is Transform transform)
                {
                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = transform.gameObject;
                        EditorGUIUtility.PingObject(transform.gameObject);
                    }
                }
                else if (value is MonoBehaviour monoBehaviour)
                {
                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = monoBehaviour.gameObject;
                        EditorGUIUtility.PingObject(monoBehaviour.gameObject);
                    }
                }
                else if (value is GameObject gameObject)
                {
                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = gameObject;
                        EditorGUIUtility.PingObject(gameObject);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unity对象选择错误: {e.Message}");
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }
    }
}
#endif
