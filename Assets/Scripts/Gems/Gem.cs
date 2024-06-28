using UnityEngine;
public class Gem : MonoBehaviour
{
    public GemData data;

    public void Initialise(GemData gemData)
    {
        data = gemData;
        // Optionally, you can instantiate any effects or additional components here
    }
}
