using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterItem : MonoBehaviour
{
    private CharacterType m_character = CharacterType.Angel;

    [SerializeField] private Image m_avatar;
    [SerializeField] private TextMeshProUGUI m_tmpMana;
    [SerializeField] private GameObject m_choice;

    //

    public CharacterType Character => m_character;

    //

    public void SetCharacter(CharacterType Character)
    {
        m_character = Character;
        m_avatar.sprite = GameManager.Instance.CharacterConfig.GetConfig(Character).Skin[0].Avartar;
    }

    public void SetMana(int Mana)
    {
        m_tmpMana.text = Mana.ToString();
    }

    public void SetChoice(CharacterType Character)
    {
        m_choice.SetActive(m_character == Character);
    }
}