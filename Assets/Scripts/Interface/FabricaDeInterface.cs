using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Plataforma.Interface
{
    public static class FabricaDeInterface
    {
        public static readonly Color CorTexto = new Color(0.93f, 0.89f, 0.76f, 1f);
        public static readonly Color CorContorno = new Color(0.06f, 0.05f, 0.04f, 1f);
        public static readonly Color CorBotao = new Color(0.30f, 0.33f, 0.18f, 1f);
        public static readonly Color CorBotaoDestaque = new Color(0.62f, 0.18f, 0.14f, 1f);
        public static readonly Color CorPainel = new Color(0.08f, 0.08f, 0.06f, 0.78f);
        public static readonly Color CorDestaque = new Color(0.95f, 0.76f, 0.25f, 1f);

        private static Font fonte;
        private static Sprite spriteArredondado;

        public static Font Fonte
        {
            get
            {
                if (fonte == null)
                {
                    fonte = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                return fonte;
            }
        }

        public static Canvas CriarCanvas(string nome, int ordemDeDesenho)
        {
            GameObject objetoTela = new GameObject(nome, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            objetoTela.layer = LayerMask.NameToLayer("UI");

            Canvas tela = objetoTela.GetComponent<Canvas>();
            tela.renderMode = RenderMode.ScreenSpaceOverlay;
            tela.sortingOrder = ordemDeDesenho;

            CanvasScaler escalador = objetoTela.GetComponent<CanvasScaler>();
            escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            escalador.referenceResolution = new Vector2(1920f, 1080f);
            escalador.matchWidthOrHeight = 0.5f;

            GarantirEventSystem();
            return tela;
        }

        public static void GarantirEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject objetoEventSystem = new GameObject("EventSystem");
            objetoEventSystem.AddComponent<EventSystem>();
            objetoEventSystem.AddComponent<InputSystemUIInputModule>();
        }

        public static RectTransform CriarRetangulo(Transform pai, string nome, Vector2 ancoraMin, Vector2 ancoraMax,
            Vector2 posicao, Vector2 tamanhoRet, Vector2 pivo)
        {
            GameObject objetoRetangulo = new GameObject(nome, typeof(RectTransform));
            objetoRetangulo.layer = pai.gameObject.layer;
            RectTransform retangulo = objetoRetangulo.GetComponent<RectTransform>();
            retangulo.SetParent(pai, false);
            retangulo.anchorMin = ancoraMin;
            retangulo.anchorMax = ancoraMax;
            retangulo.pivot = pivo;
            retangulo.anchoredPosition = posicao;
            retangulo.sizeDelta = tamanhoRet;
            return retangulo;
        }

        public static Text CriarTexto(Transform pai, string nome, string conteudo, int tamanhoFonte, TextAnchor alinhamento,
            Vector2 ancoraMin, Vector2 ancoraMax, Vector2 posicao, Vector2 tamanhoRet, Vector2 pivo)
        {
            RectTransform retangulo = CriarRetangulo(pai, nome, ancoraMin, ancoraMax, posicao, tamanhoRet, pivo);
            Text texto = retangulo.gameObject.AddComponent<Text>();
            texto.font = Fonte;
            texto.fontSize = tamanhoFonte;
            texto.alignment = alinhamento;
            texto.color = CorTexto;
            texto.text = conteudo;
            texto.supportRichText = true;
            texto.raycastTarget = false;
            texto.horizontalOverflow = HorizontalWrapMode.Wrap;
            texto.verticalOverflow = VerticalWrapMode.Overflow;

            Outline contorno = retangulo.gameObject.AddComponent<Outline>();
            contorno.effectColor = CorContorno;
            contorno.effectDistance = new Vector2(3f, -3f);
            return texto;
        }

        public static Image CriarImagem(Transform pai, string nome, Sprite sprite, Color cor,
            Vector2 ancoraMin, Vector2 ancoraMax, Vector2 posicao, Vector2 tamanhoRet, Vector2 pivo)
        {
            RectTransform retangulo = CriarRetangulo(pai, nome, ancoraMin, ancoraMax, posicao, tamanhoRet, pivo);
            Image imagem = retangulo.gameObject.AddComponent<Image>();
            imagem.sprite = sprite;
            imagem.color = cor;
            imagem.preserveAspect = sprite != null;
            imagem.raycastTarget = false;
            return imagem;
        }

        public static Image CriarPainel(Transform pai, string nome, Color cor,
            Vector2 ancoraMin, Vector2 ancoraMax, Vector2 posicao, Vector2 tamanhoRet, Vector2 pivo)
        {
            Image painel = CriarImagem(pai, nome, SpriteArredondado, cor, ancoraMin, ancoraMax, posicao,
                tamanhoRet, pivo);
            painel.type = Image.Type.Sliced;
            painel.preserveAspect = false;
            return painel;
        }

        public static Button CriarBotao(Transform pai, string rotulo, Vector2 posicao, Vector2 tamanho,
            UnityAction aoClicar)
        {
            Vector2 centro = new Vector2(0.5f, 0.5f);
            Image fundoBotao = CriarPainel(pai, "Botao " + rotulo, Color.white, centro, centro,
                posicao, tamanho, centro);
            fundoBotao.raycastTarget = true;

            Button botao = fundoBotao.gameObject.AddComponent<Button>();
            botao.targetGraphic = fundoBotao;
            ColorBlock cores = botao.colors;
            cores.normalColor = CorBotao;
            cores.highlightedColor = CorBotaoDestaque;
            cores.selectedColor = CorBotaoDestaque;
            cores.pressedColor = new Color(0.42f, 0.1f, 0.08f, 1f);
            cores.colorMultiplier = 1f;
            cores.fadeDuration = 0.08f;
            botao.colors = cores;
            if (aoClicar != null)
            {
                botao.onClick.AddListener(aoClicar);
            }

            Text texto = CriarTexto(fundoBotao.transform, "Texto", rotulo, 46, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, centro);
            texto.horizontalOverflow = HorizontalWrapMode.Overflow;
            return botao;
        }

        public static void Selecionar(Button botao)
        {
            if (botao != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(botao.gameObject);
            }
        }

        public static Sprite SpriteArredondado
        {
            get
            {
                if (spriteArredondado != null)
                {
                    return spriteArredondado;
                }

                const int tamanho = 32;
                const int raio = 10;
                Texture2D textura = new Texture2D(tamanho, tamanho, TextureFormat.RGBA32, false);
                textura.filterMode = FilterMode.Bilinear;
                textura.wrapMode = TextureWrapMode.Clamp;

                Color32[] pixels = new Color32[tamanho * tamanho];
                for (int y = 0; y < tamanho; y++)
                {
                    for (int x = 0; x < tamanho; x++)
                    {
                        float dx = Mathf.Max(0f, Mathf.Max(raio - x - 0.5f, x + 0.5f - (tamanho - raio)));
                        float dy = Mathf.Max(0f, Mathf.Max(raio - y - 0.5f, y + 0.5f - (tamanho - raio)));
                        float distancia = Mathf.Sqrt(dx * dx + dy * dy);
                        float alfa = Mathf.Clamp01(raio - distancia + 0.5f);
                        pixels[y * tamanho + x] = new Color32(255, 255, 255, (byte)(alfa * 255f));
                    }
                }
                textura.SetPixels32(pixels);
                textura.Apply();

                spriteArredondado = Sprite.Create(textura, new Rect(0, 0, tamanho, tamanho), new Vector2(0.5f, 0.5f), 100f, 0,
                    SpriteMeshType.FullRect, new Vector4(raio, raio, raio, raio));
                spriteArredondado.name = "Arredondado";
                return spriteArredondado;
            }
        }
    }
}
