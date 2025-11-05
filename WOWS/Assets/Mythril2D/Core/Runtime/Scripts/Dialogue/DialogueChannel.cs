using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public class AwaitableDialogueTree
    {
        public DialogueTree dialogue;
        public TaskCompletionSource<bool> task;
    }

    public class DialogueChannel : MonoBehaviour
    {
        // Public Events
        public UnityEvent<DialogueTree> dialogueStarted = new();
        public UnityEvent<DialogueTree> dialogueEnded = new();
        public UnityEvent<DialogueNode> dialogueNodeChanged = new();

        // Private Members
        private AwaitableDialogueTree m_currentTree = null;
        private DialogueNode m_currentNode = null;
        private Queue<AwaitableDialogueTree> m_dialogueQueue = new();

        public void Interrupt()
        {
            if (m_currentTree != null)
            {
                ClearQueue();
                OnDialogueCompleted();
            }
        }

        public TaskCompletionSource<bool> AddToQueue(DialogueTree dialogue)
        {
            Debug.Assert(dialogue != null, "Cannot enqueue a null dialogue tree.");

            var task = new TaskCompletionSource<bool>();

            m_dialogueQueue.Enqueue(new()
            {
                dialogue = dialogue,
                task = task
            });

            return task;
        }

        public void ClearQueue()
        {
            foreach (var dialogue in m_dialogueQueue)
            {
                if (!dialogue.task.Task.IsCompleted)
                {
                    dialogue.task.SetResult(false);
                }
            }

            m_dialogueQueue.Clear();
        }

        public async Task PlayNow(string line, params object[] args)
        {
            await PlayNow(new DialogueTree(new DialogueNode(StringFormatter.Format(line, args))));
        }

        public async Task PlayNow(DialogueTree dialogue)
        {
            var task = AddToQueue(dialogue);

            if (!IsPlaying())
            {
                await PlayQueue();
            }
            else
            {
                await task.Task;
            }
        }

        public async Task PlayQueue()
        {
            if (!IsPlaying())
            {
                while (m_dialogueQueue.Count > 0)
                {
                    var current = m_dialogueQueue.Dequeue();
                    Play(current);
                    await current.task.Task;
                }
            }
        }

        public bool TrySkipping()
        {
            if (m_currentNode != null && m_currentNode.optionCount < 2)
            {
                Next();
                return true;
            }

            return false;
        }

        public void Next(int option = 0)
        {
            m_currentTree.dialogue.OnNodeExecuted(m_currentNode, option);
            m_currentNode.toExecuteOnCompletion?.Execute();
            SetCurrentNode(m_currentNode.GetNext(option));
        }

        public bool IsPlaying() => m_currentTree != null;

        private void Play(AwaitableDialogueTree tree)
        {
            if (tree.dialogue.root != null)
            {
                m_currentTree = tree;
                dialogueStarted.Invoke(tree.dialogue);
                tree.dialogue.dialogueStarted.Invoke();
                SetCurrentNode(tree.dialogue.root);
            }
            else
            {
                Debug.LogError("Cannot start a dialogue with a null entry point node.");
            }
        }

        private void SetCurrentNode(DialogueNode node)
        {
            m_currentNode = node;
            dialogueNodeChanged.Invoke(m_currentNode);

            if (m_currentNode == null)
            {
                OnDialogueCompleted();
            }
            else
            {
                m_currentNode.toExecuteOnStart?.Execute();
            }
        }

        private void OnDialogueCompleted()
        {
            if (IsPlaying())
            {
                m_currentNode = null;
                dialogueEnded.Invoke(m_currentTree.dialogue);
                m_currentTree.dialogue.dialogueEnded.Invoke(m_currentTree.dialogue.messages);
                var task = m_currentTree.task;
                m_currentTree = null; // this needs to be done before setting the task result
                task.SetResult(true);
            }
        }
    }
}
