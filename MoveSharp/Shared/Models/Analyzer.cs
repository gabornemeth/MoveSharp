using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    class Analyzer
    {
        //THB will be the number of RR intervals in your array
        //MRR or I(BAR) you can calculate as follows -

        //float rrTotal=0;

        //for (int i=1;i<[rrIntervals count]; i++) {
        //   rrTotal+=[[rrIntervals objectAtIndex:i] intValue];
        //}

        //float mrr = rrTotal/([rrIntervals count]-1);
        
        //SDNN (which is your HRV) is calculated as follows -

        //float sdnnTotal = 0;

        //for (int i=1;i<[rrIntervals count]; i++) {
        //   sdnTotal+=pow( [[rrIntervals objectAtIndex:i] intValue] - mrr,2) ;
        //}

        //float sdnn = sqrt(sdnTotal/([rrIntervals count]-1));
    }
}
