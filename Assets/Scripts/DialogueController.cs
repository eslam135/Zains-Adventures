using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArabicSupport;
using System.Linq;
using System.Globalization;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea] public string[] parts;
}

public enum DialogueState { Dialogue1, Dialogue2, Dialogue3 }

public class DialogueController : MonoBehaviour
{
    public static DialogueState currentState = DialogueState.Dialogue1;

    public TMP_Text dialogueText;
    public TMP_Text speakerText;
    public UnityEngine.UI.Button nextButton;

    private List<DialogueLine> currentDialogue;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private PlayerController playerController;

    [Range(0.005f, 0.1f)]
    public float typeDelay = 0.04f;

    private void Awake()
    {
        nextButton.onClick.AddListener(ShowNextLine);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        SetDialogueByState();
        currentLineIndex = 0;
        ShowCurrentLine();
        if (playerController != null)
            playerController.SetMovementEnabled(false);

        if (dialogueText) dialogueText.isRightToLeftText = false;
        if (speakerText) speakerText.isRightToLeftText = false;
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.SetMovementEnabled(true);
    }

    private void SetDialogueByState()
    {
        switch (currentState)
        {
            case DialogueState.Dialogue1: currentDialogue = dialogue1; break;
            case DialogueState.Dialogue2: currentDialogue = dialogue2; break;
        }
    }

    private string JoinLine(DialogueLine line)
    {
        if (line?.parts == null || line.parts.Length == 0) return string.Empty;
        return string.Join("\n", line.parts.Where(p => !string.IsNullOrEmpty(p)));
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        var line = currentDialogue[currentLineIndex];
        speakerText.text = ArabicFixer.Fix(line.speaker, showTashkeel: true, useHinduNumbers: true);

        string raw = JoinLine(line);
        typingCoroutine = StartCoroutine(TypeText(raw));
    }

    private IEnumerator TypeText(string raw)
    {
        dialogueText.text = "";

        var e = StringInfo.GetTextElementEnumerator(raw);
        string acc = "";
        while (e.MoveNext())
        {
            acc += e.GetTextElement();
            dialogueText.text = ArabicFixer.Fix (acc);
            yield return new WaitForSeconds(typeDelay);
        }
    }

    public void ShowNextLine()
    {
        string fullFixed = ArabicFixer.Fix(JoinLine(currentDialogue[currentLineIndex]), showTashkeel: true, useHinduNumbers: true);

        if (typingCoroutine != null && dialogueText.text != fullFixed)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = fullFixed;
            return;
        }

        currentLineIndex++;
        if (currentLineIndex < currentDialogue.Count)
        {
            ShowCurrentLine();
        }
        else
        {
            gameObject.SetActive(false);
            AdvanceState();
        }
    }

    private void AdvanceState()
    {
        switch (currentState)
        {
            case DialogueState.Dialogue1: currentState = DialogueState.Dialogue2; break;
            case DialogueState.Dialogue2: currentState = DialogueState.Dialogue3; break;
            case DialogueState.Dialogue3: default: break;
        }
    }


    private List<DialogueLine> dialogue1 = new List<DialogueLine>
{
    new DialogueLine { speaker = "زين", parts = new [] {
        "عمي سالم!", "لماذا يهرول الناس بهذا الشكل؟", "ولماذا أرى رجال راسم في كل مكان؟"
    }},
    new DialogueLine { speaker = "سالم", parts = new [] {
        "الله وحده هو الستار يا ولدي.", "رأيت راسم الحفار وعشرات من رجاله هنا.", "وجوههم تقول بأنهم لا ينوون خيرًا."
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "أقسم أنني سأنتقم منهم شر انتقام", "إن كان ما أفكر فيه صحيحًا."
    }},
    new DialogueLine { speaker = "سالم", parts = new [] {
        "لا تحتك يا زين براسم أو غيره!", "لا تورط نفسك مع هؤلاء.", "أنت وجدك رحمه الله من أخيار هذه البلاد. لا تصبح من الحمقى."
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "سأكون من الحمقى إن لم أتحرك الآن."
    }},
    new DialogueLine { speaker = "سالم", parts = new [] {
        "إلى أين تذهب؟"
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "إلى الحارس.", "هو الوحيد الذي يمكنه مساعدتي."
    }},
    new DialogueLine { speaker = "سالم", parts = new [] {
        "فلتحرص اذا على جمع جميع العملات المفقوده ليتمكن من مساعدتك", "لتصبح آمناً يا ولدي."
    }},
};

    private List<DialogueLine> dialogue2 = new List<DialogueLine>
{
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "والآن ماذا تريد مني يا زين؟"
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "أنا واثق بأن الشيخ راسم هو الذي سرق المفتاح.", "وأنا الآن أريد ارشادك حتى أجده."
    }},
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "سأكون إلى جوارك وأبعث لك بالرسائل مع طائري.", "وكل ما عليك أن تفعله أن تعثر على الخريطة التي مزقها راسم."
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "لا أفهم."
    }},
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "راسم قد مزق الخريطة للباب الخفي.", "لقد حاول حرق الخريطة حتى لا يصل غيره إلى هناك.", "لكن هذه الخريطة كانت مُحصنة غير قابلة للحرق. فمزقها وخبأ كل جزء في مكان يصعب الوصول إليه."
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "لقد حدثني جدي عن هذا الباب.", "قال أنه باب أسرار الكون."
    }},
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "نعم، إنه باب أسرار الكون والكنوز المفقودة.", "لهذا سرق مفتاح رمسيس واختفى.", "يجب أن تصل قبل راسم يا زين، فإن وصل راسم انتهى العالم الذي نعرفه."
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "سأعثر على أجزاء الخريطة حتى أصل لهذا الباب."
    }},
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "إن فعلت هذا قضيت تمامًا على الشيخ راسم.", "لأن بفتح باب الكنز مرة أخرى ستلاحقه لعنات هذا المكان حتى لو كان في آخر العالم.", "وخذ هذه."
    }},
    new DialogueLine { speaker = "زين", parts = new [] {
        "ما هذه؟"
    }},
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "هذه قلادة كليوباترا.", "كانت تعرف من خلالها أسرار مصر واليونان معًا.", "ارتديها وخبأها في ثيابك بشكل جيد. إنها طريقتك للتواصل معي."
    }},
    new DialogueLine { speaker = "الحارس", parts = new [] {
        "من أجل أن تُفتح لك الأبواب، يجب أن تجمع ثلاثة من تماثيل الماو.", "القط \"ماو\" لطالما كان رمز الحظ والحماية والقوة.", "اذهب الآن."
    }},
        new DialogueLine { speaker = "الحارس", parts = new [] {
            "وكن حذرًا، فهناك من يتربص بك.", "يمكنك أن ترى بعضهم في الطريق.", "احدهم يملك اول تمثال ماو"
        }},
};
}
