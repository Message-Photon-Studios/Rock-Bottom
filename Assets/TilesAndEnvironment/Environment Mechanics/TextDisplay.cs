using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class TextDisplay : InteractionObject
{
    [SerializeField] PickUpCanvasController canvasController;
    [SerializeField] LocalizedString nameString;
    [SerializeField] LocalizedString[] descriptionString;

    int textNum = 0;

    protected override void PlayerClose(bool isClose)
    {
        base.PlayerClose(isClose);

        if (isClose)
        {
            textNum = 0;
            string setName = nameString.GetLocalizedString();
            string setDesc = descriptionString[textNum].GetLocalizedString();
            canvasController.SetDisplayText(setName, setDesc, descriptionString.Length - 1 <= textNum);
        }
        else
        {
            textNum = -1;
            canvasController.CloseMenu();
        }
    }

    protected override void PlayerInteract()
    {
        textNum++;
        if (textNum >= descriptionString.Length)
        {
            canvasController.CloseMenu();
            textNum = -1;
        }
        else
        {
            string setName = nameString.GetLocalizedString();
            string setDesc = descriptionString[textNum].GetLocalizedString();
            canvasController.SetDisplayText(setName, setDesc, descriptionString.Length - 1 <= textNum);
        }
    }
}
