using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialSceneScript : MonoBehaviour {
    Canvas firstScreenCanvas;
    Canvas createPetCanvas;

	// Use this for initialization
	void Start () {
        firstScreenCanvas = transform.FindChild("FirstScreenCanvas").GetComponent<Canvas>();
        createPetCanvas = transform.FindChild("CreatePetCanvas").GetComponent<Canvas>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnNewPetClicked()
    {
        firstScreenCanvas.gameObject.SetActive(false);
        createPetCanvas.gameObject.SetActive(true);
    }

    public void OnCreatePetCancelClicked()
    {
        createPetCanvas.gameObject.SetActive(false);
        firstScreenCanvas.gameObject.SetActive(true);
    }

    public void OnCreateCatClicked()
    {
        Debug.Log("Create Cat Clicked");
        SceneManager.LoadScene("CatScene");
    }

    public void OnCreateDogClicked()
    {
        Debug.Log("Create Dog Clicked");
        SceneManager.LoadScene("DogScene");
    }
}
