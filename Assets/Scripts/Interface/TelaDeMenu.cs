using Plataforma.Nucleo;
using Plataforma.Visual;
using UnityEngine;
using UnityEngine.UI;

namespace Plataforma.Interface
{
    public class TelaDeMenu : MonoBehaviour
    {
        private Button botaoJogar;

        private void Awake()
        {
            Time.timeScale = 1f;

            CenarioDeFundo.Criar(Camera.main, Color.white, ArteDoJogo.PernasCorrendo, ArteDoJogo.TroncoFrente(false));

            Canvas tela = FabricaDeInterface.CriarCanvas("Menu", 10);
            Transform raiz = tela.transform;
            Vector2 centro = new Vector2(0.5f, 0.5f);
            Vector2 topo = new Vector2(0.5f, 1f);
            Vector2 baixo = new Vector2(0.5f, 0f);

            FabricaDeInterface.CriarImagem(raiz, "Faixa Topo", null, new Color(0.05f, 0.05f, 0.04f, 0.92f), new Vector2(0f, 1f),
                Vector2.one, Vector2.zero, new Vector2(0f, 260f), topo);
            FabricaDeInterface.CriarImagem(raiz, "Filete Topo", null, new Color(0.62f, 0.12f, 0.1f, 1f), new Vector2(0f, 1f),
                Vector2.one, new Vector2(0f, -260f), new Vector2(0f, 8f), topo);
            FabricaDeInterface.CriarImagem(raiz, "Faixa Base", null, new Color(0.05f, 0.05f, 0.04f, 0.92f), Vector2.zero,
                new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, 120f), baixo);

            Text titulo = FabricaDeInterface.CriarTexto(raiz, "Titulo", "FOREST SLUG", 140, TextAnchor.MiddleCenter,
                topo, topo, new Vector2(0f, -130f), new Vector2(1800f, 170f), topo);
            titulo.color = FabricaDeInterface.CorDestaque;
            titulo.GetComponent<Outline>().effectDistance = new Vector2(6f, -6f);

            FabricaDeInterface.CriarPainel(raiz, "Briefing", FabricaDeInterface.CorPainel, centro, centro,
                new Vector2(0f, -40f), new Vector2(1180f, 300f), centro);
            FabricaDeInterface.CriarTexto(raiz, "Titulo Briefing", "BRIEFING", 34, TextAnchor.UpperLeft, centro, centro,
                new Vector2(-560f, 90f), new Vector2(400f, 50f), new Vector2(0f, 1f)).color = FabricaDeInterface.CorDestaque;
            FabricaDeInterface.CriarTexto(raiz, "Historia",
                "A Legião Negra do General Voss invadiu a floresta e capturou civis.\n" +
                "Você é o último comando da unidade. Avance pelas linhas inimigas,\n" +
                "resgate todos os reféns e destrua o tanque do General.",
                36, TextAnchor.MiddleLeft, centro, centro, new Vector2(0f, -30f), new Vector2(1120f, 170f), centro);

            botaoJogar = FabricaDeInterface.CriarBotao(raiz, "INICIAR MISSÃO", new Vector2(-260f, -280f),
                new Vector2(480f, 100f), Cenas.NovoJogo);
            FabricaDeInterface.CriarBotao(raiz, "SAIR", new Vector2(260f, -280f), new Vector2(480f, 100f), Sair);

            FabricaDeInterface.CriarTexto(raiz, "Controles",
                "<color=#f2c240>A/D</color> andar   <color=#f2c240>W/S</color> mirar   " +
                "<color=#f2c240>ESPAÇO</color> pular   <color=#f2c240>J</color> atirar   " +
                "<color=#f2c240>K</color> granada   <color=#f2c240>ESC</color> pausa",
                32, TextAnchor.MiddleCenter, baixo, baixo, new Vector2(0f, 70f), new Vector2(1800f, 50f), baixo);
        }

        private void Start()
        {
            FabricaDeInterface.Selecionar(botaoJogar);
        }

        private void Update()
        {
            if (Controles.Confirmar)
            {
                Cenas.NovoJogo();
            }
        }

        private static void Sair()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
