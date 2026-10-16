using Plataforma.Nucleo;
using UnityEngine;

namespace Plataforma.Visual
{
    [RequireComponent(typeof(Camera))]
    public class CameraSeguidora : MonoBehaviour
    {
        private const float Suavizacao = 0.15f;
        private const float Antecipacao = 1.5f;
        private const float AlturaExtra = 1.5f;

        private Transform alvo;
        private Rect limites;
        private Camera cam;
        private Vector3 velocidade;
        private float antecipacaoAtual;
        private float forcaTremor;
        private float tempoTremor;
        private Vector3 ultimoTremor;

        public void Configurar(Transform alvoSeguido, Rect limitesDoMundo)
        {
            alvo = alvoSeguido;
            limites = limitesDoMundo;
            cam = GetComponent<Camera>();
            transform.position = PosicaoDesejada();
        }

        public void Tremer(float forca, float duracao)
        {
            forcaTremor = Mathf.Max(forcaTremor, forca);
            tempoTremor = Mathf.Max(tempoTremor, duracao);
        }

        private void LateUpdate()
        {
            if (alvo == null)
            {
                return;
            }

            antecipacaoAtual = Mathf.MoveTowards(antecipacaoAtual, Controles.Horizontal * Antecipacao, 3f * Time.deltaTime);
            Vector3 posicao = Vector3.SmoothDamp(transform.position - ultimoTremor, PosicaoDesejada(), ref velocidade, Suavizacao);

            ultimoTremor = Vector3.zero;
            if (tempoTremor > 0f)
            {
                tempoTremor -= Time.deltaTime;
                ultimoTremor = (Vector3)Random.insideUnitCircle * forcaTremor;
            }
            else
            {
                forcaTremor = 0f;
            }
            transform.position = posicao + ultimoTremor;
        }

        private Vector3 PosicaoDesejada()
        {
            float meiaAltura = cam.orthographicSize;
            float meiaLargura = meiaAltura * cam.aspect;
            float x = Mathf.Clamp(alvo.position.x + antecipacaoAtual, limites.xMin + meiaLargura, limites.xMax - meiaLargura);
            float y = Mathf.Clamp(alvo.position.y + AlturaExtra, limites.yMin + meiaAltura,
                Mathf.Max(limites.yMin + meiaAltura, limites.yMax - meiaAltura));
            return new Vector3(x, y, transform.position.z);
        }
    }
}
