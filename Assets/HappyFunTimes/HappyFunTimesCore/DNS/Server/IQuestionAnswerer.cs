using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using System.Collections.Generic;

namespace DNS.Server {
    public interface IQuestionAnswerer {
        IList<IResourceRecord> Get(Question question);
    }
}


