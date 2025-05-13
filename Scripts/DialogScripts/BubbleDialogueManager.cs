using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Utilities;

namespace DialogScripts
{
    public class BubbleDialogueManager : MonoBehaviour
    {
        [SerializeField] private float repeatInterval = 5f;       // Интервал повторения диалога
        [SerializeField] private List<DialogueMapping> originalDialogueLines = new List<DialogueMapping>(); // Исходный список реплик
        private readonly Queue<DialogueMapping> _dialogueQueue = new Queue<DialogueMapping>();

        private bool _isDisplayingDialogue = false;
        private IEnumerator _dialogueLoopCoroutine;
        private IEnumerator _displayDialogueCoroutine;
        private UIBubbleDialogueController _currentSpeaker;

        private void Start()
        {
            // Наполняем очередь исходными репликами
            ResumeDialogue();
        }

        public void StopDialogue()
        {
            if (_displayDialogueCoroutine != null)
            {
                StopCoroutine(_displayDialogueCoroutine);
                _displayDialogueCoroutine = null; // Сбрасываем ссылку после остановки
            }
            if (_dialogueLoopCoroutine != null)
            {
                StopCoroutine(_dialogueLoopCoroutine);
                _dialogueLoopCoroutine = null; // Сбрасываем ссылку после остановки
            }
            _isDisplayingDialogue = false; // Сбрасываем состояние
            // Также можно скрыть текст, если необходимо
            _currentSpeaker.HideText();
        }

        public void ResumeDialogue()
        {
            ResetDialogueQueue();
            if (_dialogueLoopCoroutine != null) return; // Проверяем, не запущена ли уже корутина
            _dialogueLoopCoroutine = DialogueLoop();
            StartCoroutine(_dialogueLoopCoroutine);
        }

        // Метод для заполнения очереди диалогов исходными репликами
        private void ResetDialogueQueue()
        {
            _dialogueQueue.Clear();
            foreach (var line in originalDialogueLines)
            {
                _dialogueQueue.Enqueue(line);
            }
        }

        // Основной цикл диалога
        private IEnumerator DialogueLoop()
        {
            while (true)
            {
                if (!_isDisplayingDialogue && _dialogueQueue.Count > 0)
                {
                    _displayDialogueCoroutine = DisplayDialogue();
                    StartCoroutine(_displayDialogueCoroutine);
                }
                yield return new WaitForSeconds(repeatInterval);
            }
        }

        private IEnumerator DisplayDialogue()
        {
            _isDisplayingDialogue = true;

            while (_dialogueQueue.Count > 0)
            {
                DialogueMapping map = _dialogueQueue.Dequeue();

                // Отображаем реплику
                yield return StartCoroutine(DisplayLine(map));

                // Ждем окончания отображения текущей реплики перед переходом к следующей

            }

            // Когда все реплики отображены, восстанавливаем очередь и ждем интервал для повтора
            ResetDialogueQueue();
            _isDisplayingDialogue = false;
        }

        private IEnumerator DisplayLine(DialogueMapping map)
        {
            _currentSpeaker = map.Speaker;

            void OnLocaleChanged(Locale locale)
            {
                map.Speaker.ShowText(map.DialogueLine.text.GetLocalizedString());
            }

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

            // Отображаем текст над NPC
            map.Speaker.ShowText(map.DialogueLine.text.GetLocalizedString());

            // Ожидаем, пока истечет время отображения реплики
            yield return new WaitForSeconds(map.DialogueLine.displayTime);

            // Скрываем текст после истечения времени
            map.Speaker.HideText();

            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

            yield return new WaitForSeconds(RandomGenerator.Instance.
                RandomValueInRange(map.DelayBeforeNextDialogueLineMin, map.DelayBeforeNextDialogueLineMax));
        }
    }
    
    [Serializable]
    public struct DialogueMapping
    {
        [field: SerializeField] public UIBubbleDialogueController Speaker { get; private set; }
        [field: SerializeField] public DialogueLine DialogueLine { get; private set; }
        
        [field: SerializeField] public float DelayBeforeNextDialogueLineMin { get; private set; }
        [field: SerializeField] public float DelayBeforeNextDialogueLineMax { get; private set; }
        
    }
}
