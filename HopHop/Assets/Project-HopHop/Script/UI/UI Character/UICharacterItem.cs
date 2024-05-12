using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterItem : MonoBehaviour
{
    [SerializeField] private Image m_avatar;
    [SerializeField] private TextMeshProUGUI m_tmpMana;
    [SerializeField] private GameObject m_choice;

    private CharacterType m_character = CharacterType.Angel;

    //

    public CharacterType Character => m_character;

    //

    public void SetCharacter(CharacterType Character)
    {
        m_character = Character;
        m_avatar.sprite = CharacterManager.Instance.CharacterConfig.GetConfig(Character).Skin[0].Avartar;
    }

    public void SetMana(int Mana)
    {
        m_tmpMana.text = Mana.ToString();
    }

    public void SetChoice(bool Stage)
    {
        m_choice.SetActive(Stage);
    }
}