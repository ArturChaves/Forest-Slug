using System.Collections.Generic;
using Plataforma.Jogo;
using Plataforma.Nucleo;
using Plataforma.Visual;
using UnityEngine;
using UnityEngine.UI;

namespace Plataforma.Interface
{
    public class Hud : MonoBehaviour
    {
        private Text textoPontos;
        private Text textoRefens;
        private Text textoArma;
        private GameObject barraChefe;
        private RectTransform vidaChefe;
        private Jogador jogador;

        public TanqueChefe Chefe { get; set; }
        private Text textoMensagem;
        private Text textoSubmensagem;
        private GameObject telaPausa;
        private readonly List<Image> iconesVida = new List<Image>();
        private float tempoMensagem;

        public static Hud Criar(string tituloDaFase, Jogador jogador)
        {
            Canvas tela = FabricaDeInterface.CriarCanvas("HUD", 10);
            Hud hud = tela.gameObject.AddComponent<Hud>();
            hud.jogador = jogador;
            hud.Construir(tela.transform, tituloDaFase);
            return hud;
        }

        private void Construir(Transform raiz, string tituloDaFase)
        {
            Vector2 topoEsquerda = new Vector2(0f, 1f);
            Vector2 topoDireita = new Vector2(1f, 1f);
            Vector2 topo = new Vector2(0.5f, 1f);
            Vector2 centro = new Vector2(0.5f, 0.5f);

            FabricaDeInterface.CriarPainel(raiz, "Painel Pontos", FabricaDeInterface.CorPainel, topoEsquerda, topoEsquerda,
                new Vector2(24f, -24f), new Vector2(520f, 220f), topoEsquerda);

            textoPontos = FabricaDeInterface.CriarTexto(raiz, "Pontos", "PONTOS 000000", 44, TextAnchor.MiddleLeft,
                topoEsquerda, topoEsquerda, new Vector2(52f, -34f), new Vector2(400f, 60f), topoEsquerda);

            FabricaDeInterface.CriarImagem(raiz, "Icone Refem", ArteDoJogo.PrimeiroQuadro(ArteDoJogo.RefemAmarrado), Color.white,
                topoEsquerda, topoEsquerda, new Vector2(44f, -90f), new Vector2(64f, 64f), topoEsquerda);
            textoRefens = FabricaDeInterface.CriarTexto(raiz, "Refens", "0 / 0", 44, TextAnchor.MiddleLeft,
                topoEsquerda, topoEsquerda, new Vector2(118f, -94f), new Vector2(380f, 60f), topoEsquerda);

            FabricaDeInterface.CriarImagem(raiz, "Icone Granada", ArteDoJogo.PrimeiroQuadro(ArteDoJogo.SpriteGranada), Color.white,
                topoEsquerda, topoEsquerda, new Vector2(48f, -160f), new Vector2(56f, 56f), topoEsquerda);
            textoArma = FabricaDeInterface.CriarTexto(raiz, "Arma", string.Empty, 34, TextAnchor.MiddleLeft,
                topoEsquerda, topoEsquerda, new Vector2(118f, -160f), new Vector2(420f, 56f), topoEsquerda);

            FabricaDeInterface.CriarTexto(raiz, "Titulo", tituloDaFase, 36, TextAnchor.UpperCenter,
                topo, topo, new Vector2(0f, -30f), new Vector2(900f, 60f), topo);

            FabricaDeInterface.CriarPainel(raiz, "Painel Vidas", FabricaDeInterface.CorPainel, topoDireita, topoDireita,
                new Vector2(-24f, -24f), new Vector2(64f * SessaoDeJogo.VidasMaximas + 40f, 96f), topoDireita);
            for (int i = 0; i < SessaoDeJogo.VidasMaximas; i++)
            {
                float x = -44f - (SessaoDeJogo.VidasMaximas - 1 - i) * 64f;
                iconesVida.Add(FabricaDeInterface.CriarImagem(raiz, "Vida " + (i + 1), ArteDoJogo.IconeVida, Color.white,
                    topoDireita, topoDireita, new Vector2(x, -46f), new Vector2(56f, 56f), topoDireita));
            }

            textoMensagem = FabricaDeInterface.CriarTexto(raiz, "Mensagem", string.Empty, 84, TextAnchor.MiddleCenter,
                centro, centro, new Vector2(0f, 120f), new Vector2(1600f, 140f), centro);
            textoSubmensagem = FabricaDeInterface.CriarTexto(raiz, "Submensagem", string.Empty, 44, TextAnchor.MiddleCenter,
                centro, centro, new Vector2(0f, 30f), new Vector2(1600f, 80f), centro);

            Vector2 baixo = new Vector2(0.5f, 0f);
            barraChefe = FabricaDeInterface.CriarPainel(raiz, "Barra Chefe", FabricaDeInterface.CorPainel, baixo, baixo,
                new Vector2(0f, 40f), new Vector2(900f, 70f), baixo).gameObject;
            FabricaDeInterface.CriarTexto(barraChefe.transform, "Nome", "TANQUE DO GENERAL", 30, TextAnchor.MiddleLeft,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(20f, 0f), new Vector2(340f, 50f),
                new Vector2(0f, 0.5f));
            Image fundo = FabricaDeInterface.CriarImagem(barraChefe.transform, "Fundo", null, new Color(0.25f, 0.05f, 0.05f, 1f),
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(370f, 0f), new Vector2(500f, 30f),
                new Vector2(0f, 0.5f));
            vidaChefe = FabricaDeInterface.CriarImagem(fundo.transform, "Vida", null, new Color(0.9f, 0.2f, 0.15f, 1f),
                Vector2.zero, new Vector2(1f, 1f), Vector2.zero, Vector2.zero, new Vector2(0f, 0.5f)).rectTransform;
            barraChefe.SetActive(false);

            telaPausa = FabricaDeInterface.CriarImagem(raiz, "Pausa", null, new Color(0f, 0f, 0f, 0.55f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, centro).gameObject;
            FabricaDeInterface.CriarTexto(telaPausa.transform, "Texto Pausa",
                "PAUSA\n<size=40>Esc / P  continuar      M  voltar ao menu</size>", 90, TextAnchor.MiddleCenter,
                centro, centro, Vector2.zero, new Vector2(1600f, 300f), centro);
            telaPausa.SetActive(false);
        }

        private void Update()
        {
            textoPontos.text = "PONTOS " + SessaoDeJogo.Pontos.ToString("D6");
            for (int i = 0; i < iconesVida.Count; i++)
            {
                iconesVida[i].color = i < SessaoDeJogo.Vidas ? Color.white : new Color(0.15f, 0.1f, 0.12f, 0.5f);
            }
            if (jogador != null)
            {
                bool metralhadora = jogador.ArmaAtual == Jogador.Arma.Metralhadora;
                textoArma.text = jogador.Granadas.ToString("D2") + "    "
                                 + (metralhadora ? "METRALHADORA " + jogador.Municao : "PISTOLA ∞");
            }

            bool chefeVivo = Chefe != null && Chefe.EstaVivo;
            barraChefe.SetActive(chefeVivo);
            if (chefeVivo)
            {
                vidaChefe.anchorMax = new Vector2(Chefe.FracaoDeVida, 1f);
            }

            if (tempoMensagem > 0f)
            {
                tempoMensagem -= Time.unscaledDeltaTime;
                if (tempoMensagem <= 0f)
                {
                    textoMensagem.text = string.Empty;
                    textoSubmensagem.text = string.Empty;
                }
            }
        }

        public void MostrarRefens(int resgatados, int total)
        {
            textoRefens.text = "REFÉNS " + resgatados + " / " + total;
        }

        public void MostrarMensagem(string mensagem, string submensagem, float segundos)
        {
            textoMensagem.text = mensagem;
            textoSubmensagem.text = submensagem ?? string.Empty;
            tempoMensagem = segundos;
        }

        public void MostrarPausa(bool pausado)
        {
            telaPausa.SetActive(pausado);
        }
    }
}
