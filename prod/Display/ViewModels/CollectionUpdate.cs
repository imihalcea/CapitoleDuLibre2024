using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;

namespace Display.ViewModels;

internal class CollectionUpdate<T> where T : notnull
{
  private readonly SourceList<T> _source;
    
  public CollectionUpdate(out ReadOnlyObservableCollection<T> store)
  {
    _source = new SourceList<T>();
    _source
      .Connect()
      .Bind(out store)
      .Subscribe();
  }

  public void UpdateWith(IEnumerable<T> collection)
  {
    _source.Edit(s =>
    {
      s.Clear();
      collection.ToList().ForEach(s.Add);
    });
  }
}