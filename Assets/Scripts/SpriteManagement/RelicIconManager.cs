using UnityEngine;

public class RelicIconManager : IconManager
{
    void Start()
    {
        GameManager.Instance.relicIconManager = this;
    }

    public Sprite GetSprite(int index)
    {
        return Get(index);
    }
}
