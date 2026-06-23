using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格敵のラウンド別情報を入れたラウンド別敵の追加データ
/// </summary>
[Serializable]
public class EnemySpawnData
{
    public EnemyController EnemyPrefab; // 召喚する敵のBaseプレハブ（騎士、ゴブンリン等。。）
    public int SpawnWeight = 1;         // Spawn確率(重み）。 Round別設定（企画的柔軟さの為に）
    
    public Color EnemyColor = Color.white; // 敵のカラ
    public float StatMultiplier = 1.0f; // Stats倍率（後で数値別分離）
}

/// <summary>
/// 格ステージ別格ラウンドに登場する敵の情報が込めたSO（ステージ別）
/// 後でGoogle Sheetからデータ注入する予定
/// </summary>
[CreateAssetMenu(fileName = "New Stage Spawn Data", menuName = "Game Data/Stage Spawn Data")]
public class StageSpawnDataSO : ScriptableObject
{
    [Header("1Round Enemy Lists")]
    public List<EnemySpawnData> Round1Enemies;

    [Header("2Round Enemy Lists")]
    public List<EnemySpawnData> Round2Enemies;

    [Header("3Round Enemy Lists")]
    public List<EnemySpawnData> Round3Enemies;
}