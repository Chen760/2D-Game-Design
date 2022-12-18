using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;

    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas = FindObjectOfType<Canvas>();
    }

    private void OnEnable()
    {
        CharaterEvents.characterDamaged += CharaterTookDamage;
        CharaterEvents.characterHealed += CharaterHealed;
    }

    private void OnDisable()
    {
        CharaterEvents.characterDamaged -= CharaterTookDamage;
        CharaterEvents.characterHealed -= CharaterHealed;
    }


    public void CharaterTookDamage(GameObject character, int damageReceived)
    {
        Vector2 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();

    }

    public void CharaterHealed(GameObject character, int healthRestored)
    {
        Vector2 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                Debug.Log(this.name+":"+this.GetType()+":"+System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            #if(UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;
            #elif(UNIY_STANDALONE)
                Application.Quit();
            #elif(UNITY_WEBGL)
                SceneManager.LoadScene("QuitScene");
            #endif

        }
    }


}
