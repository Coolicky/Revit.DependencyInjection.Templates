﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit.DependencyInjection.Unity.Async;

namespace Revit.DependencyInjection.Templates.Unity.Interfaces
{
    public interface ISampleSelector
    {
        Task<IEnumerable<Element>> GetSelectedOrSelectElements();
        Task SelectElement(ElementId id);
    }

    public class SampleSelector : ISampleSelector
    {
        private readonly IRevitEventHandler _eventHandler;

        public SampleSelector(IRevitEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public async Task<IEnumerable<Element>> GetSelectedOrSelectElements()
        {
            return await _eventHandler.RunAsync(GetSelection);
        }

        public async Task SelectElement(ElementId id)
        {
            await _eventHandler.RunAsync(uiApp => AddSingleToSelection(uiApp, new List<ElementId>() { id }));
        }

        private void AddSingleToSelection(UIApplication uiApp, ICollection<ElementId> elementIds)
        {
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            uiDoc.Selection.SetElementIds(elementIds);
        }

        private IEnumerable<Element> GetSelection(UIApplication uiApp)
        {
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var currentSelection = uiDoc.Selection.GetElementIds();
            if (currentSelection.Any())
                return currentSelection.Select(r => doc.GetElement(r));

            var selection = uiDoc.Selection.PickObjects(ObjectType.Element);
            return selection.Select(r => doc.GetElement(r));
        }
    }
}