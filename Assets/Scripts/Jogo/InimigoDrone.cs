using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(AnimadorDeSprite))]
    public class InimigoDrone : InimigoBase
    {
        private AnimadorDeSprite animador;
        private Vector2 origem;
        private float alcance = 3f;
        private float rapidez = 1.8f;
        private float defasagem;
        private float esperaBomba = 1f;
        private float velocidadeQueda;
        private float tempoMorte;

        protected override int Pontos => 150;
        protected override bool Sangra => false;

        protected override void Awake()
        {
            base.Awake();
            animador = GetComponent<AnimadorDeSprite>();
            areaDeAcerto.size = new Vector2(0.6f, 0.55f);
            areaDeAcerto.offset = new Vector2(0f, -0.05f);
            animador.Tocar(ArteDoJogo.DroneVoando, 18f);
            animador.Embaralhar();
        }

        public void Configurar(Vector2 inicio, float alcancePatrulha, float velocidadeVoo)
        {
            origem = inicio;
            alcance = Mathf.Max(0.5f, alcancePatrulha);
            rapidez = velocidadeVoo;
            defasagem = Random.value * Mathf.PI * 2f;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            if (morto)
            {
                velocidadeQueda += 18f * dt;
                corpo.MovePosition(corpo.position + Vector2.down * velocidadeQueda * dt);
                tempoMorte -= dt;
                if (tempoMorte <= 0f)
                {
                    Explosao.Criar(corpo.position, 0f, Equipe.Inimigo, 0, 1f);
                    Destroy(gameObject);
                }
                return;
            }

            float t = Time.time * (rapidez / alcance) + defasagem;
            float x = origem.x + Mathf.Sin(t) * alcance;
            float y = origem.y + Mathf.Sin(Time.time * 2.5f + defasagem) * 0.3f;
            corpo.MovePosition(new Vector2(x, y));

            esperaBomba -= dt;
            Jogador jogador = JogadorAlvo();
            if (jogador != null && esperaBomba <= 0f && NaTela(corpo.position))
            {
                Vector2 delta = (Vector2)jogador.transform.position - corpo.position;
                if (Mathf.Abs(delta.x) < 1.2f && delta.y < -0.5f)
                {
                    Projetil.Disparar(ArteDoJogo.Bomba, corpo.position + Vector2.down * 0.4f, new Vector2(0f, -1f),
                        Equipe.Inimigo, 1, 4f, 14f, 1.1f);
                    esperaBomba = 1.8f;
                }
            }
        }

        protected override void AoSerDerrotado()
        {
            animador.Tocar(ArteDoJogo.DroneDestruido, 1f, false);
            tempoMorte = 0.6f;
        }
    }
}
