using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginEditor.GameProject.Utilites
{
    /// <summary>
    /// Define which Command can implement undo redo action
    /// </summary>
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private Action _undoAction;
        private Action _redoAction; 
        public string Name { get; }

        public void Redo() => _redoAction();
        
        public void Undo() => _undoAction();

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
            Name = name;
        }

    }

    public class UndoRedo
    {
        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
        public  ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

        public void Reset()
        {
            _redoList.Clear();
            _undoList.Clear();
        }

        public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
        }

        public void Add(IUndoRedo cmd)
        {
            _undoList.Add(cmd);
            _redoList.Clear();
        }

        public void Undo()
        {
            if (_undoList.Any())
            { 
                var cmd = _undoList.Last(); //get the last command excuted
                _undoList.RemoveAt(_undoList.Count-1); //Remove it
                cmd.Undo();// Undo the command
                _redoList.Insert(0, cmd);

            }
        }
        public void Redo()
        {
            if (RedoList.Any())
            {
                var cmd = _redoList.First();
                _redoList.RemoveAt(0);
                cmd.Redo();
                _undoList.Add(cmd);
            }
        }

    }
}
