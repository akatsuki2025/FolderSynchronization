using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Validators
{
    public interface IAccessValidator
    {
        void ValidateReadAccess(string path);
        void ValidateWriteAccess(string path);
    }
}
