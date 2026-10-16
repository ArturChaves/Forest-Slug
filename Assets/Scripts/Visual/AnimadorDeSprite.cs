using System.Linq;
using UnityEngine;

namespace Plataforma.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimadorDeSprite : MonoBehaviour
    {
        private Sprite[] quadros;
        private float quadrosPorSegundo = 8f;
        private bool repetir = true;
        private float tempo;
        private int indice;

        public bool Terminou { get; private set; }

        public void Tocar(Sprite[] novosQuadros, float fps, bool deveRepetir = true)
        {
            if (novosQuadros == null || novosQuadros.Length == 0)
            {
                return;
            }

            quadrosPorSegundo = Mathf.Max(0.01f, fps);
            repetir = deveRepetir;
            if (quadros != null && quadros.SequenceEqual(novosQuadros))
            {
                return;
            }

            quadros = novosQuadros;
            indice = 0;
            tempo = 0f;
            Terminou = false;
            GetComponent<SpriteRenderer>().sprite = quadros[0];
        }

        public void Embaralhar()
        {
            if (quadros != null)
            {
                indice = Random.Range(0, quadros.Length);
                GetComponent<SpriteRenderer>().sprite = quadros[indice];
            }
        }

        private void Update()
        {
            if (quadros == null || quadros.Length < 2 || Terminou)
            {
                return;
            }

            tempo += Time.deltaTime;
            if (tempo < 1f / quadrosPorSegundo)
            {
                return;
            }

            tempo = 0f;
            indice++;
            if (indice >= quadros.Length)
            {
                if (repetir)
                {
                    indice = 0;
                }
                else
                {
                    indice = quadros.Length - 1;
                    Terminou = true;
                }
            }
            GetComponent<SpriteRenderer>().sprite = quadros[indice];
        }
    }
}
