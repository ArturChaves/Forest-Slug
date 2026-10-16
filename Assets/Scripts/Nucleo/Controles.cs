using UnityEngine;
using UnityEngine.InputSystem;

namespace Plataforma.Nucleo
{
    public static class Controles
    {
        public static float Horizontal
        {
            get
            {
                float eixo = 0f;
                if (Segurando(Key.A, Key.LeftArrow))
                {
                    eixo -= 1f;
                }
                if (Segurando(Key.D, Key.RightArrow))
                {
                    eixo += 1f;
                }

                Gamepad controle = Gamepad.current;
                if (eixo == 0f && controle != null)
                {
                    float analogico = controle.leftStick.x.ReadValue();
                    if (Mathf.Abs(analogico) > 0.35f)
                    {
                        eixo = Mathf.Sign(analogico);
                    }
                    else if (controle.dpad.left.isPressed)
                    {
                        eixo = -1f;
                    }
                    else if (controle.dpad.right.isPressed)
                    {
                        eixo = 1f;
                    }
                }
                return eixo;
            }
        }

        public static bool MirarCima => Segurando(Key.W, Key.UpArrow)
                                        || (Gamepad.current != null && (Gamepad.current.dpad.up.isPressed
                                                                        || Gamepad.current.leftStick.y.ReadValue() > 0.6f));

        public static bool MirarBaixo => Segurando(Key.S, Key.DownArrow)
                                         || (Gamepad.current != null && (Gamepad.current.dpad.down.isPressed
                                                                         || Gamepad.current.leftStick.y.ReadValue() < -0.6f));

        public static bool PuloApertado => Apertou(Key.Space, Key.C)
                                           || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

        public static bool PuloSegurado => Segurando(Key.Space, Key.C)
                                           || (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed);

        public static bool AtirarApertado => Apertou(Key.J, Key.Z, Key.LeftCtrl)
                                             || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                                             || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);

        public static bool AtirarSegurado => Segurando(Key.J, Key.Z, Key.LeftCtrl)
                                             || (Mouse.current != null && Mouse.current.leftButton.isPressed)
                                             || (Gamepad.current != null && Gamepad.current.buttonWest.isPressed);

        public static bool GranadaApertada => Apertou(Key.K, Key.X)
                                              || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);

        public static bool Confirmar => Apertou(Key.Enter, Key.NumpadEnter)
                                        || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);

        public static bool PausaApertada => Apertou(Key.Escape, Key.P)
                                            || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);

        public static bool MenuApertado => Apertou(Key.M);

        private static bool Segurando(params Key[] teclas)
        {
            Keyboard teclado = Keyboard.current;
            if (teclado == null)
            {
                return false;
            }
            foreach (Key tecla in teclas)
            {
                if (teclado[tecla].isPressed)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Apertou(params Key[] teclas)
        {
            Keyboard teclado = Keyboard.current;
            if (teclado == null)
            {
                return false;
            }
            foreach (Key tecla in teclas)
            {
                if (teclado[tecla].wasPressedThisFrame)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
