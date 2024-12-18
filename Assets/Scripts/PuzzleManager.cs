using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static Mecanismo;

public class PuzzleManager : MonoBehaviour
{
  
    public Mecanismo[] plataformasPuzzle;
    public GameObject gancho;
    int blendShapeIndex = 0; 
    SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {
        skinnedMeshRenderer = gancho.GetComponent<SkinnedMeshRenderer>();
        foreach (var puzzle in plataformasPuzzle)
        {
            puzzle.posicaoInicial = puzzle.plataforma.transform.position;
        }
    }

    private void Update()
    {

        Mover();
    }
    void MoveGancho()
    {
        float currentBlendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);

        if (currentBlendShapeWeight < 100)
        {
            float newBlendShapeWeight = Mathf.Min(currentBlendShapeWeight + (17 * Time.deltaTime), 100);
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, newBlendShapeWeight);
        }
    }

    public void BotaoPressionado(int ordemDoBotao, ButtonScript scriptBotao, int indicePlataforma)
    {
        var puzzle = plataformasPuzzle[indicePlataforma];
        if (ordemDoBotao == puzzle.ordemCorreta[puzzle.passoAtual] && LanternaPlayer.lanternaPlayer.naPosicao)
        {
            puzzle.passoAtual++;
            puzzle.alavancasAtivadas.Add(scriptBotao);
            if (puzzle.passoAtual == puzzle.ordemCorreta.Length)
            {
                AudioController.audioController.PlaySoundEffectAtIndex(2);
                AtivarPlataforma(indicePlataforma);
            }
        }
        else
        {
            ResetarPuzzle(indicePlataforma);
            scriptBotao.AtivarEEsperar();
            AudioController.audioController.PlaySoundEffectAtIndex(9);
        }
    }

    private void AtivarPlataforma(int indicePlataforma)
    {
        var puzzle = plataformasPuzzle[indicePlataforma];
        puzzle.plataformaAtiva = true;
    }

    private void ResetarPuzzle(int indicePlataforma)
    {
        var puzzle = plataformasPuzzle[indicePlataforma];
        puzzle.passoAtual = 0;
        foreach (ButtonScript botao in puzzle.alavancasAtivadas)
        {
            botao.DesativarBotao();
        }
        puzzle.alavancasAtivadas.Clear();
    }

    private void ResetarTodosOsPuzzles()
    {
        foreach (var puzzle in plataformasPuzzle)
        {
            puzzle.passoAtual = 0;
            foreach (ButtonScript botao in puzzle.alavancasAtivadas)
            {
                botao.DesativarBotao();
            }
            puzzle.alavancasAtivadas.Clear();
            puzzle.plataforma.transform.position = Vector3.MoveTowards(puzzle.plataforma.transform.position, puzzle.posicaoInicial, puzzle.velocidade * Time.deltaTime);
        }
    }

    void Mover()
    {
        if (GameController.controller.uiController.visivelpause == true) return;

        foreach (var puzzle in plataformasPuzzle)
        {
            switch (puzzle.tipo)
            {
                case 1:
                    if (puzzle.plataformaAtiva)
                    {
                        Vector3 destino = puzzle.movendoParaPosicaoFinal ? puzzle.posicaoFinal : puzzle.posicaoInicial;
                        puzzle.plataforma.transform.position = Vector3.MoveTowards( puzzle.plataforma.transform.position,destino, puzzle.velocidade * Time.deltaTime);

                        if (Vector3.Distance(puzzle.plataforma.transform.position, destino) < 0.01f)
                        {
                            puzzle.movendoParaPosicaoFinal = !puzzle.movendoParaPosicaoFinal;
                        }
                    }
                    break;

                case 2:
                    if (puzzle.plataformaAtiva)
                    {
                        puzzle.plataforma.transform.position = Vector3.MoveTowards( puzzle.plataforma.transform.position,puzzle.posicaoFinal,puzzle.velocidade * Time.deltaTime);

                        if (Vector3.Distance(puzzle.plataforma.transform.position, puzzle.posicaoFinal) < 0.01f)
                        {
                            puzzle.plataformaAtiva = false;
                        }
                        MoveGancho();
                    }
                    break;

                default:
                    break;
            }

            if (!LanternaPlayer.lanternaPlayer.naPosicao)
            {
                ResetarTodosOsPuzzles();
            }
        }
    }
}