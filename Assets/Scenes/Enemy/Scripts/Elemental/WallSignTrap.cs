using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallSignTrap : MonoBehaviour
{
    public List<GameObject> signs;
    [ColorUsage(true, true)]
    public Color glowColor;
    private Color startColor;
    public float intensutyGlow;

    public List<float> maskSize;
    public List<SpriteRenderer> rends;
    private MaterialPropertyBlock propBlock;
    private MaterialPropertyBlock colorPropBlock;
    void Awake()
    {
        foreach (var rend in signs)
        {
            rends.Add(rend.GetComponent<SpriteRenderer>());
        }
        propBlock = new MaterialPropertyBlock();
        colorPropBlock = new MaterialPropertyBlock();

        var sr = signs[0].GetComponent<SpriteRenderer>();
        var block = new MaterialPropertyBlock();
        sr.GetPropertyBlock(block);
        startColor = block.GetColor("_GlowColor");
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < rends.Count && i < maskSize.Count; i++)
        {
            rends[i].GetPropertyBlock(propBlock);
            propBlock.SetFloat("_MaskSize", maskSize[i]); // використовуємо відповідний індекс
            rends[i].SetPropertyBlock(propBlock);
        }

        foreach (var sing in signs)
        {
            sing.transform.eulerAngles = new Vector3(0, 0, Random.Range(1f, 360f));
        }
    }

    public void GlowSing(GameObject rotatedObj,bool active)
    {
        if (active)
        {
            rotatedObj.GetComponent<SpriteRenderer>().GetPropertyBlock(colorPropBlock);
            colorPropBlock.SetColor("_GlowColor", glowColor); // використовуємо відповідний індекс
            rotatedObj.GetComponent<SpriteRenderer>().SetPropertyBlock(colorPropBlock);
        }
        else
        {
            rotatedObj.GetComponent<SpriteRenderer>().GetPropertyBlock(colorPropBlock);
            colorPropBlock.SetColor("_GlowColor", startColor); // використовуємо відповідний індекс
            rotatedObj.GetComponent<SpriteRenderer>().SetPropertyBlock(colorPropBlock);
        }
        
    }
}
