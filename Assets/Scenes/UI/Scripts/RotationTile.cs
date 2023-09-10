using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RotationTile : MonoBehaviour
{
    public Tilemap tilemap; // Посилання на Tilemap, де використовуються тайли
    public List<Image> floor; // Посилання на Tilemap, де використовуються тайли
    public Quaternion[] rotationOptions; // Масив з різними ротаціями для тайлів
    public bool isTileMap;
    void Start()
    {
        if (isTileMap)
        {
            BoundsInt bounds = tilemap.cellBounds;

            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(position);

                // Перевіряємо, чи тайл не є порожнім
                if (tile != null)
                {
                    // Отримуємо випадкове значення для вибору ротації
                    int randomValue = Random.Range(0, 2);

                    Quaternion rotation;

                    // Вибираємо ротацію на основі випадкового значення
                    if (randomValue == 0)
                    {
                        rotation = Quaternion.Euler(0f, 0f, 0f); // 0 градусів
                    }
                    else
                    {
                        rotation = Quaternion.Euler(0f, 0f, 180f); // 180 градусів
                    }

                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
                    tilemap.SetTransformMatrix(position, matrix);
                }
            }
        }
        else
        {
            foreach (Image spriteRenderer in floor)
            {
                int randomValue = Random.Range(0, 2);

                Quaternion rotation;

                if (randomValue == 0)
                {
                    rotation = Quaternion.Euler(0f, 0f, 0f); // 0 градусів
                }
                else
                {
                    rotation = Quaternion.Euler(0f, 0f, 180f); // 180 градусів
                }

                spriteRenderer.transform.rotation = rotation;
            }
        }
    }
}


