using UnityEngine;

namespace DatabaseSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Level")]
    public class LevelData : ScriptableObject
    {
        #region Variables
        [SerializeField] private int _id = default;
        [Header("Слово \"Уровень\" перед цифрой")]
        [SerializeField] private string _nameKey = default;
        [Header("Префаб уровня")]
        [SerializeField] private GameObject _prefabMobile;
        [Header("Монет за прохождение уровня")]
        [SerializeField] private int _coinsForLevel;
        [Header("Множитель на здоровье животных")]
        [SerializeField] private float _enemyHealthMultiplier;
        [Header("Тип уровня и цели")]
        [SerializeField] private HoldingType _levelType;
        [Header("Позиция START POINT")]
        [SerializeField] private Vector3 _positionStartPoint;
        [SerializeField] private Quaternion _rotationStartPoint;

        public int Id { get => _id; set => _id = value; }
        public string NameKey { get => _nameKey; set => _nameKey = value; }
        public int CoinsForLevel { get => _coinsForLevel; set => _coinsForLevel = value; }
        public float EnemyHealthMultiplier { get => _enemyHealthMultiplier; set => _enemyHealthMultiplier = value; }
        public HoldingType LevelType { get => _levelType; set => _levelType = value; }
        public GameObject PrefabMobile { get => _prefabMobile; set => _prefabMobile = value; }
        public Vector3 PositionStartPoint { get => _positionStartPoint; set => _positionStartPoint = value; }
        public Quaternion RotationStartPoint { get => _rotationStartPoint; set => _rotationStartPoint = value; }
        #endregion
    }
}
