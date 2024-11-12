using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrowMat_Script : MonoBehaviour
{
    public List<MeshRenderer> growMeshes;
    public float timeToGrow = 5;
    public float refreshRate = 0.05f;
    public float damage = 0.5f;
    public float damageTick = 0.5f;
    [Range(0, 1)]
    public float minGrow = 0f;
    [Range(0, 1)]
    public float maxGrow = 1f;
    public Transform trailParent;

    private List<Material> growMaterials = new List<Material>();
    private Material currentMat;
    private bool fullyGrown;
    // Start is called before the first frame update
    void Start()
    {
        //trailParent = GetComponent<VeinPool>().trailParent;

        for (int i = 0; i< growMeshes.Count;i++)
        {
            for (int j = 0; j < growMeshes[i].materials.Length;j++)
            {
                if (growMeshes[i].materials[j].HasProperty("_Grow"))
                {
                    growMeshes[i].materials[j].SetFloat("_Grow", minGrow);
                    growMaterials.Add(growMeshes[i].materials[j]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i<growMaterials.Count; i++)
        {
            StartCoroutine(Grow(growMaterials[i]));
            currentMat = growMaterials[i];
        }
    }
    IEnumerator Grow(Material mat)
    {
        
        float growValue = mat.GetFloat("_Grow");
        if (!fullyGrown)
        {
            while (growValue < maxGrow)
            {
                growValue += 1 / (timeToGrow / refreshRate);
                mat.SetFloat("_Grow", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrow)
            {
                growValue -= 1 / (timeToGrow / refreshRate);
                mat.SetFloat("_Grow", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
            GetRotationAndPosition();
        }
        fullyGrown = growValue >= maxGrow ? true : false;
    }
    public void GetRotationAndPosition()
    {
        gameObject.SetActive(false);
        if (currentMat != null)
        {
            currentMat.SetFloat("_Grow", 0);
        }
        if (trailParent != null)
        {
            transform.position = trailParent.position;
        }
        gameObject.SetActive(true);

        Rigidbody2D playerRb = PlayerManager.instance.rb;
        if (playerRb.velocity.magnitude > 0)
        {
            float angle = Mathf.Atan2(playerRb.velocity.y, playerRb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.x + angle + Random.Range(-90, 90));
        }
    }
}
