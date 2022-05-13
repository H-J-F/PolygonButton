using UIExtensions;
using UnityEditor;
using UnityEngine;


public static class PolygonUIMenu
{
    [MenuItem("GameObject/UI/CircleImage", priority = 2010)]
    public static void CreateCircleImage()
    {
        var selectGo = Selection.activeGameObject;

        var circleImgGo = new GameObject("CircleImage");
        circleImgGo.transform.SetParent(selectGo.transform);
        circleImgGo.AddComponent<CircleImage>();
        circleImgGo.layer = selectGo == null ? LayerMask.NameToLayer("UI") : selectGo.layer;

        var circleCollider = circleImgGo.GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        Selection.activeGameObject = circleImgGo;
    }

    [MenuItem("GameObject/UI/CircleRawImage", priority = 2011)]
    public static void CreateCircleRawImage()
    {
        var selectGo = Selection.activeGameObject;

        var circleRawImgGo = new GameObject("CircleRawImage");
        circleRawImgGo.transform.SetParent(selectGo.transform);
        circleRawImgGo.AddComponent<CircleRawImage>();
        circleRawImgGo.layer = selectGo == null ? LayerMask.NameToLayer("UI") : selectGo.layer;

        var circleCollider = circleRawImgGo.GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        Selection.activeGameObject = circleRawImgGo;
    }

    [MenuItem("GameObject/UI/PolygonImage", priority = 2012)]
    public static void CreatePolygonImage()
    {
        var selectGo = Selection.activeGameObject;

        var polygonImgGo = new GameObject("PolygonImage");
        polygonImgGo.transform.SetParent(selectGo.transform);
        polygonImgGo.AddComponent<PolygonImage>();
        polygonImgGo.layer = selectGo == null ? LayerMask.NameToLayer("UI") : selectGo.layer;

        var circleCollider = polygonImgGo.GetComponent<PolygonCollider2D>();
        circleCollider.isTrigger = true;

        Selection.activeGameObject = polygonImgGo;
    }

    [MenuItem("GameObject/UI/PolygonRawImage", priority = 2013)]
    public static void CreatePolygonRawImage()
    {
        var selectGo = Selection.activeGameObject;

        var polygonRawImgGo = new GameObject("PolygonRawImage");
        polygonRawImgGo.transform.SetParent(selectGo.transform);
        polygonRawImgGo.AddComponent<PolygonRawImage>();
        polygonRawImgGo.layer = selectGo == null ? LayerMask.NameToLayer("UI") : selectGo.layer;

        var circleCollider = polygonRawImgGo.GetComponent<PolygonCollider2D>();
        circleCollider.isTrigger = true;

        Selection.activeGameObject = polygonRawImgGo;
    }

    [MenuItem("GameObject/UI/UI Polygon", priority = 2014)]
    public static void CreateUIPolygon()
    {
        var selectGo = Selection.activeGameObject;

        var uiPolygon = new GameObject("UI Polygon");
        uiPolygon.transform.SetParent(selectGo.transform);
        uiPolygon.AddComponent<UIPolygon>();
        uiPolygon.layer = selectGo == null ? LayerMask.NameToLayer("UI") : selectGo.layer;

        Selection.activeGameObject = uiPolygon;
    }
}