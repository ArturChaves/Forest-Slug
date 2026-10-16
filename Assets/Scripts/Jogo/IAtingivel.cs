using UnityEngine;

namespace Plataforma.Jogo
{
    public enum Equipe
    {
        Jogador,
        Inimigo
    }

    public interface IAtingivel
    {
        Equipe Equipe { get; }
        bool EstaVivo { get; }
        void ReceberDano(int quantidade, Vector2 origemDano);
    }
}
