using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DungeonPainter : EditorWindow
    {
        private float _tileSize;
        private float _height;
        private GameObject _prefab;
        private Transform _rootTransform;
        private bool _isPainting;
        private GameObject _cursorObject;
        private Vector3 _latestPos;
        private List<Vector2> _activeTiles;

        [MenuItem("Window/ProjectTools/DungeonPainter")]
        public static void ShowWindow()
        {
            GetWindow<DungeonPainter>("DungeonPainter");
        }


        private void OnGUI()
        {
            EditorGUILayout.Space();
            DrawTileSizeField();
            EditorGUILayout.Space();
            DrawHeightField();
            EditorGUILayout.Space();
            DrawPrefabField();
            EditorGUILayout.Space();
            DrawRootTransformField();
            EditorGUILayout.Space();
            DrawEnablePaintingButton();
            DrawDisablePaintingButton();
        }

        private void DrawTileSizeField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile size", GUILayout.MaxWidth(128));
            _tileSize = EditorGUILayout.FloatField(_tileSize);
            GUILayout.EndHorizontal();
        }
        private void DrawHeightField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Height", GUILayout.MaxWidth(128));
            _height = EditorGUILayout.FloatField(_height);
            GUILayout.EndHorizontal();
        }
        private void DrawPrefabField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefab", GUILayout.MaxWidth(128));
            _prefab = (GameObject)EditorGUILayout.ObjectField(_prefab,
                typeof(GameObject), true);
            GUILayout.EndHorizontal();
        }

        private void DrawRootTransformField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root transform", GUILayout.MaxWidth(128));
            _rootTransform = (Transform)EditorGUILayout.ObjectField(_rootTransform,
                typeof(Transform), true);
            GUILayout.EndHorizontal();
        }

        private void DrawEnablePaintingButton()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable painting"))
            {
                EnablePainting();
                Debug.Log("Painting enabled");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDisablePaintingButton()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Disable painting"))
            {
                DisablePainting();
                Debug.Log("Painting disabled");
            }
            GUILayout.EndHorizontal();
        }

        private void EnablePainting()
        {
            _cursorObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Cursor"));
            //SetCursorPosition();
            ScanActiveTiles();
            _isPainting = true;
        }

        private void DisablePainting()
        {
            GameObject.DestroyImmediate(_cursorObject);
            _isPainting = false;
        }

        private void SetCursorPosition()
        {
            Ray ray = Camera.current.ScreenPointToRay(
                new Vector3(Camera.current.pixelWidth / 2, Camera.current.pixelHeight / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject target = hit.transform.gameObject;

                Vector3 hitPos = hit.point;
                Debug.Log(target.gameObject.tag);
                if (target != null)
                {
                    hitPos = hitPos + Vector3.up * _prefab.transform.localScale.y / 2;
                    float x = GetGridedPosition(hitPos.x);
                    float z = GetGridedPosition(hitPos.z);
                    hitPos = new Vector3(x, hitPos.y, z);
                    _cursorObject.transform.position = hitPos;
                }
            }
        }

        private void ScanActiveTiles()
        {
            _activeTiles = new List<Vector2>();
            float currentHeight = 0;
            //float currentHeight = _cursorObject.transform.position.y;
            int startX = 0;
            int startY = 0;
            for (int i = startX - 100; i < startX + 100; i++)
            {
                for (int j = startY - 100; j < startY + 100; j++)
                {
                    Vector2 gridedPosition = new Vector2(i, j);
                    Vector2 position = new Vector2(gridedPosition.x * _tileSize, gridedPosition.y * _tileSize);
                    if (CheckPositionOccupied(position, currentHeight))
                    {
                        _activeTiles.Add(gridedPosition);
                    }
                }
            }
        }

        private bool CheckPositionOccupied(Vector2 position, float currentHeight)
        {
            bool isOccupied = false;
            Vector3 rayOrigin = new Vector3(position.x, currentHeight + (_prefab.transform.localScale.y * 3), position.y);
            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject target = hit.transform.gameObject;
                if (target != null && target.gameObject.tag != "EditorOnly")
                {
                    isOccupied = true;
                    //Debug.Log($"{position} is occupied");
                }
            }
            return isOccupied;
        }

        private void Update()
        {
            if (_isPainting)
            {
                Ray ray = Camera.current.ScreenPointToRay(
                    new Vector3(Camera.current.pixelWidth / 2, Camera.current.pixelHeight / 2, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject target = hit.transform.gameObject;

                    Vector3 hitPos = hit.point;
                    Debug.Log(target.gameObject.tag);
                    if (target != null && target.gameObject.tag == "EditorOnly")
                    {
                        hitPos = hitPos + Vector3.up * _prefab.transform.localScale.y / 2;
                        float x = GetGridedPosition(hitPos.x);
                        float z = GetGridedPosition(hitPos.z);
                        hitPos = new Vector3(x, hitPos.y, z);
                        _cursorObject.transform.position = hitPos;
                        if (!CheckPosChanged(hitPos))
                        {
                            return;
                        }
                        Vector2 gridPos = new Vector2(x / _tileSize, z / _tileSize);
                        if (_activeTiles.Contains(gridPos))
                        {
                            //Debug.Log("Place occupied");
                            return;
                        }
                        if (_prefab != null)
                        {
                            //Debug.Log($"Creating at {gridPos}");
                            _activeTiles.Add(gridPos);
                            if (_rootTransform != null)
                            {
                                GameObject.Instantiate(_prefab, _cursorObject.transform.position, Quaternion.identity, _rootTransform);
                            }
                            else
                            {
                                GameObject.Instantiate(_prefab, _cursorObject.transform.position, Quaternion.identity);
                            }
                        }
                    }
                }
            }
        }

        private bool CheckPosChanged(Vector3 hitPos)
        {
            bool isChanged = true;
            if (_latestPos != null)
            {
                if (_latestPos != hitPos)
                {
                    isChanged = true;
                }
                else
                {
                    isChanged = false;
                }
            }
            _latestPos = hitPos;
            return isChanged;
        }

        private float GetGridedPosition(float position)
        {
            float additionalPos = position % _tileSize;
            position = position - additionalPos;
            if (additionalPos < _tileSize / 2)
            {
                additionalPos = 0;
            }
            else
            {
                additionalPos = _tileSize;
            }
            position = position + additionalPos;
            return position;
        }

    }
}

