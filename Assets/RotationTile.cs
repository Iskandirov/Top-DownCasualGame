using UnityEngine;
using UnityEngine.Tilemaps;


public class RotationTile : MonoBehaviour
{
    public Tilemap tilemap; // Посилання на Tilemap, де використовуються тайли

    public Quaternion[] rotationOptions; // Масив з різними ротаціями для тайлів

    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);

            // Перевіряємо, чи тайл не є порожнім
            if (tile != null)
            {
                // Отримуємо випадковий індекс ротації з масиву rotationOptions
                int randomIndex = Random.Range(0, rotationOptions.Length);
                Quaternion randomRotation = rotationOptions[randomIndex];

                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, randomRotation, Vector3.one);
                tilemap.SetTransformMatrix(position, matrix);
            }
        }
    }
}


