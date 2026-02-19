using UnityEngine;
using TMPro;
using Antymology.Terrain;

public class SimHUD : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        text.text = $"Nest Blocks: {WorldManager.Instance.NestCount}";
    }
}
