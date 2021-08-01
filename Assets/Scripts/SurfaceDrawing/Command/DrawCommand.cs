using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandUndoRedo;

namespace VRPainting
{
    public class DrawCommand : ICommand
    {
        GameObject DrawLine;

        public DrawCommand(GameObject drawLine)
        {
            this.DrawLine = drawLine;
        }

        public void Execute()
        {
            if (DrawLine != null)
            {
                DrawLine.SetActive(true);
            }
        }

        public void UnExecute()
        {
            if (DrawLine != null)
            {
                DrawLine.SetActive(false);
            }
        }
    }

    public class DeleteCommand : ICommand
    {
        GameObject DeleteLine;

        public DeleteCommand(GameObject deleteLine)
        {
            this.DeleteLine=deleteLine;
        }

        public void Execute()
        {
            if (DeleteLine != null)
            {
               DeleteLine.SetActive(false);
            }
        }

        public void UnExecute()
        {
            if (DeleteLine != null)
            {
                DeleteLine.SetActive(true);
            }
        }
    }
}
