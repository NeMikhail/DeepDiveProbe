using System;
using Player;
using UnityEngine;

namespace GameCoreModule
{
    public class PlayerEventBus
    {
        private Action<GameObject> _tryInteractWithObject;
        private Action _onTriggerSpawnLine;
        private Action _onTriggerLineExit;
        private Action _onAddOxygen;
        private Action<int> _onChangeLayer;
        private Action<int> _onOxygenChanged;
        private Action<int> _onDepthChanged;
        private Action<ObstacleView> _onInteractWithObstacle;
        private Action<StageID> _onStageChanged;
        private Action _onExtraLifeAdded;
        private Action _onExtraLifeRemoved;
        private Action _onActivateNightvision;
        
        
        public Action<GameObject> OnTryInteractWithObject
        { get => _tryInteractWithObject; set => _tryInteractWithObject = value; }
        public Action OnTriggerSpawnLine 
        { get => _onTriggerSpawnLine; set => _onTriggerSpawnLine = value; }
        public Action OnTriggerLineExit
        { get => _onTriggerLineExit; set => _onTriggerLineExit = value; }
        public Action OnAddOxygen
        { get => _onAddOxygen; set => _onAddOxygen = value; }
        public Action<int> OnChangeLayer
        { get => _onChangeLayer; set => _onChangeLayer = value; }
        public Action<int> OnOxygenChanged
        { get => _onOxygenChanged; set => _onOxygenChanged = value; }
        public Action<int> OnDepthChanged
        { get => _onDepthChanged; set => _onDepthChanged = value; }
        public Action<ObstacleView> OnInteractWithObstacle
        { get => _onInteractWithObstacle; set => _onInteractWithObstacle = value; }
        public Action<StageID> OnStageChanged
        { get => _onStageChanged; set => _onStageChanged = value; }
        public Action OnExtraLifeAdded
        { get => _onExtraLifeAdded; set => _onExtraLifeAdded = value; }
        public Action OnExtraLifeRemoved
        { get => _onExtraLifeRemoved; set => _onExtraLifeRemoved = value; }
        public Action OnActivateNightvision
        { get => _onActivateNightvision; set => _onActivateNightvision = value; }
    }
}