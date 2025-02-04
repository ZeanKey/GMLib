namespace GMLib.GM;

public static class Asset
{
    public static string GetName<TAsset>(Handle<TAsset> index) where TAsset : GMLib.Asset, IHandleable<TAsset>
    {
        return GML.Asset.GetManager<TAsset>().Assets.FirstOrDefault(pair => pair.Value == index).Key;
    }
}