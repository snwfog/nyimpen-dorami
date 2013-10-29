using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodFight
{
  public interface KnockableOpponent
  {
    int KnockOutPenaltyScore();
    int KnockOutInterval();
  }
}
