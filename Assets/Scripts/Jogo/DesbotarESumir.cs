using UnityEngine;

namespace Plataforma.Jogo
{
    public class DesbotarESumir : MonoBehaviour
    {
        private float espera;
        private float duracaoDesbote;
        private float tempo;
        private SpriteRenderer[] desenhos;

        public void Configurar(float segundosEspera, float segundosDesbote)
        {
            espera = segundosEspera;
            duracaoDesbote = Mathf.Max(0.01f, segundosDesbote);
            desenhos = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            tempo += Time.deltaTime;
            if (tempo < espera)
            {
                return;
            }

            float alfa = 1f - (tempo - espera) / duracaoDesbote;
            if (alfa <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            if (desenhos == null)
            {
                return;
            }
            foreach (SpriteRenderer r in desenhos)
            {
                if (r != null)
                {
                    Color c = r.color;
                    c.a = alfa;
                    r.color = c;
                }
            }
        }
    }
}
