using Plataforma.Nucleo;
using Plataforma.Visual;
using UnityEngine;
using UnityEngine.UI;

namespace Plataforma.Interface
{
    public class TelaFinal : MonoBehaviour
    {
        private Button botaoReiniciar;

        private void Awake()
        {
            Time.timeScale = 1f;
            bool vitoria = SessaoDeJogo.Resultado == ResultadoDoJogo.Vitoria;

            if (vitoria)
            {
                CenarioDeFundo.Criar(Camera.main, Color.white, ArteDoJogo.PernasCorrendo, ArteDoJogo.TroncoCima(false));
            }
            else
            {
                CenarioDeFundo.Criar(Camera.main, new Color(0.75f, 0.45f, 0.45f, 1f), new[] { ArteDoJogo.JogadorFerido }, null);
            }

            Canvas tela = FabricaDeInterface.CriarCanvas("Fim", 10);
            Transform raiz = tela.transform;
            Vector2 centro = new Vector2(0.5f, 0.5f);

            FabricaDeInterface.CriarPainel(raiz, "Painel", FabricaDeInterface.CorPainel, centro, centro,
                new Vector2(0f, 40f), new Vector2(1100f, 760f), centro);

            Text titulo = FabricaDeInterface.CriarTexto(raiz, "Resultado", vitoria ? "MISSÃO CUMPRIDA" : "MISSÃO FRACASSADA", 120,
                TextAnchor.MiddleCenter, centro, centro, new Vector2(0f, 320f), new Vector2(1400f, 160f), centro);
            titulo.color = vitoria ? FabricaDeInterface.CorDestaque : new Color(0.9f, 0.25f, 0.2f, 1f);
            titulo.fontSize = 104;
            titulo.GetComponent<Outline>().effectDistance = new Vector2(5f, -5f);

            string historia = vitoria
                ? "VITÓRIA! Todos os reféns foram resgatados e o tanque do\nGeneral Voss virou sucata. A floresta está livre."
                : "DERROTA. Você caiu em combate antes do fim da operação.\nVolte para a linha de frente, soldado!";
            FabricaDeInterface.CriarTexto(raiz, "Mensagem", historia, 40, TextAnchor.MiddleCenter, centro, centro,
                new Vector2(0f, 185f), new Vector2(1000f, 120f), centro);

            string estatisticas =
                $"Pontuação final: <color=#ffe08a>{SessaoDeJogo.Pontos}</color>\n" +
                $"Reféns resgatados: <color=#ffe08a>{SessaoDeJogo.Refens}</color>\n" +
                $"Medalhas: <color=#ffe08a>{SessaoDeJogo.Medalhas}</color>\n" +
                $"Inimigos derrotados: <color=#ffe08a>{SessaoDeJogo.InimigosDerrotados}</color>\n" +
                $"Missão alcançada: <color=#ffe08a>{SessaoDeJogo.UltimaFase} de {SessaoDeJogo.QuantidadeDeFases}</color>";
            FabricaDeInterface.CriarTexto(raiz, "Resumo", estatisticas, 40, TextAnchor.MiddleCenter, centro, centro,
                new Vector2(0f, 10f), new Vector2(1000f, 260f), centro);

            botaoReiniciar = FabricaDeInterface.CriarBotao(raiz, "JOGAR NOVAMENTE", new Vector2(0f, -160f),
                new Vector2(620f, 110f), Cenas.NovoJogo);
            FabricaDeInterface.CriarBotao(raiz, "MENU", new Vector2(0f, -290f), new Vector2(620f, 100f), Cenas.CarregarMenu);
        }

        private void Start()
        {
            FabricaDeInterface.Selecionar(botaoReiniciar);
        }

        private void Update()
        {
            if (Controles.Confirmar)
            {
                Cenas.NovoJogo();
            }
        }
    }
}
