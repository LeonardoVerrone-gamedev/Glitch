using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<Move> unlockedMoves;
    public Move rightButtonFastAbility;
    public Move leftButtonFastAbility;
    public int lifes;
    //informação de em qual fase o player está
    //informação de quais habilidades estão setadas como favoritas
}