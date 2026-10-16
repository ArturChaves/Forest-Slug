using Plataforma.Fases;
using UnityEngine;

namespace Plataforma.Jogo
{
    public static class FabricaDeEntidades
    {
        public static Jogador CriarJogador(Vector2 posicao, Transform pai)
        {
            GameObject objetoJogador = CriarObjeto("Soldado (Jogador)", posicao, pai);
            return objetoJogador.AddComponent<Jogador>();
        }

        public static SoldadoInimigo CriarSoldado(Vector2 posicao, MapaDaFase mapa, bool parado, float rapidez,
            float intervaloTiro, Transform pai)
        {
            GameObject objetoInimigo = CriarObjeto(parado ? "Atirador" : "Soldado Inimigo", posicao, pai);
            SoldadoInimigo soldado = objetoInimigo.AddComponent<SoldadoInimigo>();
            soldado.Configurar(mapa, parado, rapidez, intervaloTiro);
            return soldado;
        }

        public static InimigoDrone CriarDrone(Vector2 posicao, float rapidez, Transform pai)
        {
            GameObject objetoDrone = CriarObjeto("Drone", posicao, pai);
            InimigoDrone drone = objetoDrone.AddComponent<InimigoDrone>();
            drone.Configurar(posicao, 3f, rapidez);
            return drone;
        }

        public static TanqueChefe CriarTanqueChefe(Vector2 centroCelula, Transform pai)
        {
            Vector2 posicao = new Vector2(centroCelula.x, centroCelula.y + 0.5f);
            GameObject objetoTanque = CriarObjeto("Tanque (Chefe)", posicao, pai);
            return objetoTanque.AddComponent<TanqueChefe>();
        }

        public static Refem CriarRefem(Vector2 posicao, Transform pai)
        {
            GameObject objetoRefem = CriarObjeto("Refem", posicao, pai);
            return objetoRefem.AddComponent<Refem>();
        }

        public static Item CriarItem(Vector2 posicao, TipoDeItem tipo, Transform pai)
        {
            GameObject objetoItem = CriarObjeto("Item " + tipo, posicao, pai);
            Item item = objetoItem.AddComponent<Item>();
            item.Configurar(tipo);
            return item;
        }

        private static GameObject CriarObjeto(string nome, Vector2 posicao, Transform pai)
        {
            GameObject criado = new GameObject(nome);
            criado.transform.SetParent(pai, false);
            criado.transform.position = posicao;
            return criado;
        }
    }
}
