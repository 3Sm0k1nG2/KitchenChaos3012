using System;
using UnityEngine.SceneManagement;

//public sealed class Loader
//{
//    private static readonly Lazy<Loader> lazy = new Lazy<Loader>(() => new Loader());
//    public static Loader Instance
//    {
//        get { return lazy.Value; }
//    }

//    private Loader() { }

//    public enum Scene
//    {
//        MainMenuScene,
//        GameScene,
//        LoadingScene
//    }
    
//    private Scene targetScene;

//    public void LoadScene(Scene targetScene)
//    {
//        this.targetScene = targetScene;

//        SceneManager.LoadScene(Scene.LoadingScene.ToString());
//    }

//    public void LoaderCallback()
//    {
//        SceneManager.LoadScene(targetScene.ToString());
//    }
//}

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }

    private static Scene targetScene;

    public static void LoadScene(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
