using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projetil : MonoBehaviour
    {
        public const int OrdemDeDesenho = 15;

        private static readonly RaycastHit2D[] Acertos = new RaycastHit2D[16];

        private Equipe equipe;
        private Vector2 velocidade;
        private float gravidade;
        private int dano;
        private float duracao;
        private float raioExplosao;

        public static Projetil Disparar(Sprite[] quadros, Vector2 posicao, Vector2 velocidade, Equipe equipe, int dano,
            float duracao, float gravidade = 0f, float raioExplosao = 0f)
        {
            GameObject objetoProjetil = new GameObject(equipe == Equipe.Jogador ? "Bala" : "Bala Inimiga");
            objetoProjetil.transform.position = posicao;
            SpriteRenderer desenho = objetoProjetil.AddComponent<SpriteRenderer>();
            desenho.sprite = ArteDoJogo.PrimeiroQuadro(quadros);
            desenho.sortingOrder = OrdemDeDesenho;

            Projetil projetil = objetoProjetil.AddComponent<Projetil>();
            projetil.equipe = equipe;
            projetil.velocidade = velocidade;
            projetil.gravidade = gravidade;
            projetil.dano = dano;
            projetil.duracao = duracao;
            projetil.raioExplosao = raioExplosao;
            projetil.Orientar();
            return projetil;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            Vector2 posicao = transform.position;
            Vector2 passo = velocidade * dt;
            float distancia = passo.magnitude;

            if (distancia > 0.0001f)
            {
                ContactFilter2D filtro = new ContactFilter2D();
                filtro.useTriggers = true;
                int quantos = Physics2D.Raycast(posicao, passo / distancia, filtro, Acertos, distancia);
                for (int i = 0; i < quantos; i++)
                {
                    Collider2D outro = Acertos[i].collider;
                    if (outro == null || outro.GetComponentInParent<Granada>() != null)
                    {
                        continue;
                    }

                    IAtingivel alvo = outro.GetComponentInParent<IAtingivel>();
                    if (alvo != null)
                    {
                        if (alvo.Equipe == equipe || !alvo.EstaVivo)
                        {
                            continue;
                        }
                        Impacto(Acertos[i].point, alvo);
                        return;
                    }

                    if (outro.isTrigger)
                    {
                        continue;
                    }

                    Impacto(Acertos[i].point, null);
                    return;
                }
            }

            transform.position = posicao + passo;
            velocidade.y -= gravidade * dt;
            Orientar();

            duracao -= dt;
            if (duracao <= 0f)
            {
                if (raioExplosao > 0f)
                {
                    Impacto(transform.position, null);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private void Orientar()
        {
            if (velocidade.sqrMagnitude > 0.0001f)
            {
                float angulo = Mathf.Atan2(velocidade.y, velocidade.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angulo);
            }
        }

        private void Impacto(Vector2 ponto, IAtingivel alvo)
        {
            if (raioExplosao > 0f)
            {
                Explosao.Criar(ponto, raioExplosao, equipe, dano, 1f);
            }
            else
            {
                if (alvo != null)
                {
                    alvo.ReceberDano(dano, ponto - velocidade.normalized * 0.3f);
                }
                else
                {
                    Efeito.Criar(ArteDoJogo.Explosao, ponto, 30f, OrdemDeDesenho + 1, 0.3f);
                }
            }
            Destroy(gameObject);
        }
    }
}
