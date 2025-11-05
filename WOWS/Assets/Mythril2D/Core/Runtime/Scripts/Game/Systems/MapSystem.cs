using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public class MapLoadingDelegationParams
    {
        public Action<Action> unloadDelegate;
        public Action<Action> loadDelegate;
        public Action completionDelegate;
    }

    [Serializable]
    public class MapDataBlock : DataBlock
    {
        [SerializeReference, SubclassSelector] public ICheckpoint[] checkpoints;
        [HideInInspector] public string currentMap;
        [HideInInspector] public bool playtest;
    }

    public class MapSystem : AGameSystem, IDataBlockHandler<MapDataBlock>
    {
        [Header("Settings")]
        [SerializeField] private bool m_delegateTransitionResponsability = false;

        private string m_currentMap = string.Empty;
        private Stack<ICheckpoint> m_checkpointStack;

        public void SetActiveMap(string map)
        {
            SceneManager.SetActiveScene(
                SceneManager.GetSceneByName(
                    map
                )
            );

            m_currentMap = map;
        }

        public void SaveCheckpoint(ICheckpoint checkpoint)
        {
            Debug.Assert(checkpoint.IsValid(), "Invalid checkpoint data! The checkpoint will not be saved.");
            checkpoint.UpdateMapName(); // Auto-generate the map name to the current map if it's set to "Current".
            Debug.Log($"Saving checkpoint from map '{checkpoint.map}' at position: {checkpoint.position}...");
            m_checkpointStack.Push(checkpoint);
        }

        public string GetCurrentMapName()
        {
            return m_currentMap;
        }

        public bool HasCurrentMap()
        {
            return m_currentMap != string.Empty;
        }

        public void RequestTransition(string map, Action onMapUnloaded = null, Action onMapLoaded = null, Action onCompletion = null)
        {
            GameManager.NotificationSystem.mapTransitionStarted.Invoke();

            if (m_delegateTransitionResponsability)
            {
                DelegateTransition(map, onMapUnloaded, onMapLoaded, onCompletion);
            }
            else
            {
                ExecuteTransition(map, onMapUnloaded, onMapLoaded, onCompletion);
            }
        }

        private void DelegateTransition(string map, Action onMapUnloaded = null, Action onMapLoaded = null, Action onCompletion = null)
        {
            Action<Action> unloadAction =
                HasCurrentMap() && m_currentMap != map && !string.IsNullOrEmpty(map) ?
                (callback) => UnloadMap(m_currentMap, callback + onMapUnloaded) :
                (callback) =>
                {
                    callback?.Invoke();
                    onMapUnloaded?.Invoke();
                };

            Action<Action> loadAction =
                !string.IsNullOrEmpty(map) ?
                (callback) => LoadMap(map, () =>
                {
                    callback?.Invoke();
                    onMapLoaded?.Invoke();
                }) :
                (callback) =>
                {
                    callback?.Invoke();
                    onMapLoaded?.Invoke();
                };

            Action completeAction =
                () => CompleteTransition(onCompletion);

            GameManager.NotificationSystem.mapTransitionDelegationRequested.Invoke(new MapLoadingDelegationParams
            {
                unloadDelegate = unloadAction,
                loadDelegate = loadAction,
                completionDelegate = completeAction
            });
        }

        private void ExecuteTransition(string map, Action onMapUnloaded = null, Action onMapLoaded = null, Action onCompletion = null)
        {
            Action loadAction =
                () => LoadMap(map,
                (() => CompleteTransition(onCompletion)) + onMapLoaded);

            if (HasCurrentMap() && m_currentMap != map)
            {
                UnloadMap(m_currentMap, onMapUnloaded);
            }
            else
            {
                loadAction();
            }
        }

        private void UnloadMap(string map, Action onCompletion)
        {
            if (!string.IsNullOrEmpty(map) && map == m_currentMap)
            {
                Debug.Log($"Unloading Map {map}...");

                GameManager.NotificationSystem.mapUnloading.Invoke();

                AsyncOperation operation = SceneManager.UnloadSceneAsync(map);

                operation.completed += (op) =>
                {
                    GameManager.NotificationSystem.mapUnloaded.Invoke();
                    onCompletion?.Invoke();
                };
            }
            else
            {
                GameManager.NotificationSystem.mapUnloaded.Invoke();
                onCompletion?.Invoke();
            }
        }

        private void LoadMap(string map, Action onCompletion)
        {
            if (!string.IsNullOrEmpty(map) && map != m_currentMap)
            {
                Debug.Log($"Loading Map {map}...");

                GameManager.NotificationSystem.mapLoading.Invoke();

                AsyncOperation operation = SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive);

                operation.completed += (op) =>
                {
                    SetActiveMap(map);
                    GameManager.NotificationSystem.mapLoaded.Invoke();
                    onCompletion?.Invoke();
                };
            }
            else
            {
                GameManager.NotificationSystem.mapLoaded.Invoke();
                onCompletion?.Invoke();
            }
        }

        public void RespawnPlayer()
        {
            ICheckpoint checkpoint = FindValidCheckpoint();
            Debug.Assert(checkpoint != null && checkpoint.IsValid(), "No valid checkpoint found! The player cannot respawn.");
            TeleportTo(checkpoint, GameManager.Player.Revive);
        }

        private void CompleteTransition(Action onCompletion)
        {
            GameManager.NotificationSystem.mapTransitionCompleted.Invoke();
            onCompletion?.Invoke();
        }

        private ICheckpoint FindValidCheckpoint()
        {
            ICheckpoint checkpoint;

            while (m_checkpointStack.Count > 0)
            {
                checkpoint = m_checkpointStack.Peek();

                if (checkpoint.IsValid())
                {
                    return checkpoint;
                }
                else
                {
                    Debug.LogWarning("Invalid checkpoint data found! Skipping...");
                    m_checkpointStack.Pop();
                }
            }

            return null;
        }

        private ICheckpoint FindPlaytestCheckpoint()
        {
            MapInfo mapInfo = FindAnyObjectByType<MapInfo>();
            Debug.Assert(mapInfo != null, "No MapInfo object found in the scene! Did you forget to add one?");
            Debug.Assert(mapInfo.playtestCheckpoint.IsValid(), "Invalid playtest checkpoint data! Did you forget to set it?");
            Debug.Assert(string.IsNullOrEmpty(mapInfo.playtestCheckpoint.map), "Playtest checkpoint should not have a map set, as the current map should be used!");
            return mapInfo.playtestCheckpoint;
        }

        public void TeleportTo(ICheckpoint checkpoint, Action onMapLoaded = null, Action onCompletion = null)
        {
            Debug.Assert(checkpoint.IsValid(), "Invalid checkpoint data!");

            RequestTransition(checkpoint.map, null, () =>
            {
                GameManager.Player.TeleportTo(checkpoint.position);
                onMapLoaded?.Invoke();
            }, onCompletion);
        }

        public void TeleportToPlaytestStartPosition(string map, Action onCompletion = null)
        {
            RequestTransition(map, null, () =>
            {
                ICheckpoint checkpoint = FindPlaytestCheckpoint();
                SaveCheckpoint(checkpoint);
                GameManager.Player.TeleportTo(checkpoint.position);
            }, onCompletion);
        }

        public MapDataBlock CreateDataBlock()
        {
            return new MapDataBlock
            {
                currentMap = m_currentMap,
                checkpoints = m_checkpointStack.ToArray(),
            };
        }

        public void LoadDataBlock(MapDataBlock block)
        {
            m_checkpointStack = new Stack<ICheckpoint>(block.checkpoints.Reverse());

            if (block.playtest)
            {
                TeleportToPlaytestStartPosition(block.currentMap);
            }
            else
            {
                bool isFirstTimePlaying = string.IsNullOrEmpty(block.currentMap);

                if (isFirstTimePlaying)
                {
                    ICheckpoint checkpoint = FindValidCheckpoint();
                    Debug.Assert(checkpoint != null, "No valid checkpoint set in the save file! Did you forget to add one or specify a valid map & identifier?");
                    TeleportTo(checkpoint);
                }
                else
                {
                    RequestTransition(block.currentMap);
                }
            }
        }
    }
}
