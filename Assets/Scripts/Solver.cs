using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solver : MonoBehaviour
{
    public PuzzleDefinition PuzzleDefinition;
    public bool warnNoSolutions;

    public static double ComputeDifficultyScaled(double branchingFactor, int solutionDepth)
    {
        if (solutionDepth == 0) return 0;
        if (branchingFactor <= 0) return double.MaxValue;

        double difficulty = Math.Log(1 + Math.Pow(1 + (1 / branchingFactor), solutionDepth * solutionDepth));

        return Math.Round(difficulty, 2);
    }

    [ContextMenu("Solve Puzzle")]
    public void SolvePuzzle()
    {
        warnNoSolutions = true;
        var solution = Solve(PuzzleDefinition.ActorPrefabs, PuzzleDefinition.BoatSize);
        if (solution.path != null)
        {
            Debug.LogWarning($"Solution found in {solution.path.Count()} steps {solution.difficulty}");
            for (int i = 0; i < solution.path.Count; i++)
            {
                GameState state = solution.path[i];
                Debug.Log($"step{i} {state.cachedKey}");
            }
        }
    }

    public (List<GameState> path, double difficulty) Solve(List<Actor> actorPrefabs, int boatSize)
    {
        var game = FindObjectOfType<Game>(true);
        game.ClearAllActors();

        List<Actor> puzzleActors = new();
        foreach (var actorPrefab in actorPrefabs)
        {
            var newActor = Instantiate(actorPrefab);
            puzzleActors.Add(newActor);
        }

        var initialState = new GameState(
            puzzleActors.ToArray(),
            new Actor[0],
            true);
        var goalState = new GameState(
            new Actor[0],
            puzzleActors.ToArray(),
            false);
        var goalStateKey = goalState.cachedKey;

        var solution = BFS(initialState, goalStateKey, boatSize);

        game.ClearAllActors();

        if (solution.path == null && warnNoSolutions)
        {
            Debug.LogWarning("No solution found.");
        }
        if (solution.path == null)
        {
            return (null, 0);
        }

        return (solution.path, ComputeDifficultyScaled(solution.branchingFactor, solution.path.Count()));
    }

    private bool DFS(GameState currentState, string goalStateKey, int boatSize, HashSet<string> visited, List<GameState> path)
    {
        var currentStateKey = currentState.cachedKey;

        // Check if we have already visited this state
        if (visited.Contains(currentStateKey))
            return false;

        // Add the current state to the path and mark it as visited
        path.Add(currentState);
        visited.Add(currentStateKey);

        // If we have reached the goal state, return true
        if (currentStateKey == goalStateKey)
        {
            return true;
        }

        // Recursively search the next possible states
        foreach (var next in GetNextStates(currentState, boatSize))
        {
            if (DFS(next.nextState, goalStateKey, boatSize, visited, path))
            {
                return true; // Solution found, no need to explore further
            }
        }

        // If no solution is found, backtrack by removing the current state from the path
        path.RemoveAt(path.Count - 1);
        return false;
    }

    private (List<GameState> path, double branchingFactor) BFS(GameState initialState, string goalStateKey, int boatSize)
    {
        Queue<(GameState state, List<GameState> path)> queue = new();
        HashSet<string> visited = new();

        int expandedNodes = 0;
        int generatedNodes = 0;

        queue.Enqueue((initialState, new List<GameState> { initialState }));
        visited.Add(initialState.cachedKey);

        while (queue.Count > 0)
        {
            var (currentState, path) = queue.Dequeue();
            expandedNodes++; // Count expanded nodes

            if (currentState.cachedKey == goalStateKey)
            {
                double branchingFactor = expandedNodes > 0 ? (double)generatedNodes / expandedNodes : 0;
                return (path, branchingFactor);
            }

            List<GameState> nextStates = new();
            foreach (var next in GetNextStates(currentState, boatSize))
            {
                if (visited.Add(next.nextState.cachedKey)) // HashSet.Add returns true if new
                {
                    nextStates.Add(next.nextState);
                    queue.Enqueue((next.nextState, new List<GameState>(path) { next.nextState }));
                }
            }

            generatedNodes += nextStates.Count; // Count total generated nodes
        }

        return (null, 0); // No solution found
    }

    private static IEnumerable<(string key, GameState nextState)> GetNextStates(GameState state, int boatCapacity)
    {
        var currentSide = state.BoatIsLeft ? state.LeftSide : state.RightSide;
        var otherSide = state.BoatIsLeft ? state.RightSide : state.LeftSide;

        foreach (var move in GetPossibleMoves(currentSide, boatCapacity))
        {
            if (!move.Any(x => x.CanPilotBoat))
            {
                continue;
            }

            var newLeft = state.BoatIsLeft
                ? RemoveActors(state.LeftSide, move)
                : state.LeftSide.Concat(move).ToArray();

            var newRight = state.BoatIsLeft
                ? state.RightSide.Concat(move).ToArray()
                : RemoveActors(state.RightSide, move);

            if (IsValid(newLeft, newRight, move))
            {
                var nextState = new GameState(newLeft, newRight, !state.BoatIsLeft);
                yield return (nextState.cachedKey, nextState);
            }
            else
            {
                var failState = new GameState(newLeft, newRight, !state.BoatIsLeft);
                //Debug.LogError(failState.ToKey());
            }
        }
    }

    private static Actor[] RemoveActors(Actor[] original, IEnumerable<Actor> toRemove)
    {
        var list = original.ToList();
        foreach (var actor in toRemove)
        {
            list.Remove(actor);
        }
        return list.ToArray();
    }

    private static bool IsValid(IEnumerable<Actor> left, IEnumerable<Actor> right, IEnumerable<Actor> boat)
    {
        bool leftValid = !left.Any(x => x.IsGameOver(left, right, boat, out _));
        bool rightValid = !right.Any(x => x.IsGameOver(left, right, boat, out _));

        return leftValid && rightValid;
    }

    private static IEnumerable<IEnumerable<Actor>> GetPossibleMoves(Actor[] side, int boatCapacity)
    {
        return GetCombinations(side, boatCapacity);
    }

    private static IEnumerable<IEnumerable<Actor>> GetCombinations(Actor[] actors, int maxSize)
    {
        int n = actors.Length;

        for (int size = 1; size <= Math.Min(maxSize, n); size++)
        {
            foreach (var combination in GetCombinationsRecursive(actors, size, 0, new List<Actor>()))
            {
                yield return combination;
            }
        }
    }

    private static IEnumerable<IEnumerable<Actor>> GetCombinationsRecursive(Actor[] actors, int size, int start, List<Actor> current)
    {
        if (current.Count == size)
        {
            yield return current.ToArray();
            yield break;
        }

        for (int i = start; i < actors.Length; i++)
        {
            current.Add(actors[i]);
            foreach (var combination in GetCombinationsRecursive(actors, size, i + 1, current))
            {
                yield return combination;
            }
            current.RemoveAt(current.Count - 1);
        }
    }
    public static string ToKey(List<Actor> actors)
    {
        return string.Join(',', actors.Select(x => x.ActorName).OrderBy(x => x));
    }
}

public struct GameState
{
    public Actor[] LeftSide;
    public Actor[] RightSide;
    public bool BoatIsLeft;

    public readonly string cachedKey;

    public GameState(Actor[] leftSide, Actor[] rightSide, bool boatIsLeft)
    {
        this.LeftSide = leftSide.OrderBy(x => x.ActorName).ToArray();
        this.RightSide = rightSide.OrderBy(x => x.ActorName).ToArray();
        this.BoatIsLeft = boatIsLeft;

        if (LeftSide == null) LeftSide = Array.Empty<Actor>();
        if (RightSide == null) RightSide = Array.Empty<Actor>();

        var left = string.Join(',', LeftSide.Select(x => x.ActorName).OrderBy(x => x));
        var right = string.Join(',', RightSide.Select(x => x.ActorName).OrderBy(x => x));

        cachedKey = $"{left}|{right}|{(BoatIsLeft ? "L" : "R")}";
    }
}