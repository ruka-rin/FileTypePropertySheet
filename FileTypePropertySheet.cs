using SharpShell.Attributes;
using SharpShell.SharpPropertySheet;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FileTypePropertySheet
{

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class FileTypePropertySheet : SharpPropertySheet
    {
        protected override bool CanShowSheet() => SelectedItemPaths.Count() == 1;

        protected override IEnumerable<SharpPropertyPage> CreatePages()
        {
            return new[] { new FileTypePropertyPage() };
        }
    }
}
