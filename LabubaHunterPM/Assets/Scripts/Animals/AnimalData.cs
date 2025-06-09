using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animal")]
public class AnimalData : ScriptableObject
{
    [Header("������")]
    [SerializeField] private Sprite _icon;
    [Header("������ �� ������")]
    [SerializeField] private bool _canHear;
    [Header("����� �� ������")]
    [SerializeField] private bool _mustSeePlayer;
    [Header("�������� ������")]
    [SerializeField] private float _walkSpeed;
    [Header("�������� ����")]
    [SerializeField] private float _runSpeed;
    [Header("�������� �����")]
    [SerializeField] private float _attackSpeed;

    [Header("�����")]
    [SerializeField] private AudioClip _soundAttack;
    [SerializeField] private AudioClip _soundIdle;
    [SerializeField] private AudioClip _soundDeath;
    [SerializeField] private AudioClip _soundDamage;

    public bool CanHear { get => _canHear; set => _canHear = value; }
    public bool MustSeePlayer { get => _mustSeePlayer; set => _mustSeePlayer = value; }
    public float WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
    public float RunSpeed { get => _runSpeed; set => _runSpeed = value; }
    public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
    public Sprite Icon { get => _icon; set => _icon = value; }
    public AudioClip SoundAttack { get => _soundAttack; set => _soundAttack = value; }
    public AudioClip SoundIdle { get => _soundIdle; set => _soundIdle = value; }
    public AudioClip SoundDeath { get => _soundDeath; set => _soundDeath = value; }
    public AudioClip SoundDamage { get => _soundDamage; set => _soundDamage = value; }
}

