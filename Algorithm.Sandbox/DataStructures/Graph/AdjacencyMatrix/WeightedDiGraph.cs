﻿using System;
using System.Collections.Generic;

namespace Algorithm.Sandbox.DataStructures.Graph.AdjacencyMatrix
{


    //TODO implement IEnumerable & make sure duplicates are handled correctly if its not already
    /// <summary>
    /// A weighted graph implementation using dynamically growinng/shrinking adjacency matrix array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    public class WeightedDiGraph<T, W> where W : IComparable
    {
        public int VerticesCount => currentSize;

        private Dictionary<T, int> vertexIndices;
        private Dictionary<int, T> reverseVertexIndices;

        private W[,] matrix;

        private int maxSize;
        private int currentSize;


        /// <summary>
        /// Constructor
        /// </summary>
        public WeightedDiGraph()
        {
            maxSize = 1;
            vertexIndices = new Dictionary<T, int>();
            reverseVertexIndices = new Dictionary<int, T>();
            matrix = new W[maxSize, maxSize];
        }

        /// <summary>
        /// Add a new vertex to this graph
        /// O(1) complexity
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void AddVertex(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (vertexIndices.ContainsKey(value))
            {
                throw new Exception("Vertex exists.");
            }

            if (currentSize == maxSize)
            {
                doubleMatrixSize();
            }

            vertexIndices.Add(value, currentSize);
            reverseVertexIndices.Add(currentSize, value);

            currentSize++;

        }

        /// <summary>
        /// remove the given vertex
        /// O(V) complexity
        /// </summary>
        /// <param name="value"></param>
        public void RemoveVertex(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (!vertexIndices.ContainsKey(value))
            {
                throw new Exception("Vertex does'nt exist.");
            }

            if (currentSize == maxSize / 2)
            {
                halfMatrixSize();
            }

            var index = vertexIndices[value];
           
            //clear edges
            for (int i = 0; i < maxSize; i++)
            {
                matrix[i, index] = default(W);
                matrix[index, i] = default(W);
            }

            reverseVertexIndices.Remove(index);
            vertexIndices.Remove(value);

            currentSize--;

        }

        /// <summary>
        /// Add a new edge to this graph
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="weight"></param>
        public void AddEdge(T source, T dest, W weight)
        {
            if (weight.Equals(default(W)))
            {
                throw new Exception("Cannot add default edge weight.");
            }

            if (source == null || dest == null)
            {
                throw new ArgumentException();
            }

            if (!vertexIndices.ContainsKey(source) || !vertexIndices.ContainsKey(dest))
            {
                throw new Exception("Source or destination vertex does'nt exist.");
            }

            var sourceIndex = vertexIndices[source];
            var destIndex = vertexIndices[dest];
            if (!matrix[sourceIndex, destIndex].Equals(default(W)))
            {
                throw new Exception("Edge already exists.");
            }

            matrix[sourceIndex, destIndex] = weight;
        }

        /// <summary>
        /// remove the given edge from this graph
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public void RemoveEdge(T source, T dest)
        {
            if (source == null || dest == null)
            {
                throw new ArgumentException();
            }

            if (!vertexIndices.ContainsKey(source) || !vertexIndices.ContainsKey(dest))
            {
                throw new Exception("Source or destination vertex does'nt exist.");
            }

            var sourceIndex = vertexIndices[source];
            var destIndex = vertexIndices[dest];
            if (matrix[sourceIndex, destIndex].Equals(default(W)))
            {
                throw new Exception("Edge do not exists.");
            }

            matrix[sourceIndex, destIndex] = default(W);

        }

        /// <summary>
        /// do we have an edge between given source and destination?
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool HasEdge(T source, T dest)
        {
            if (source == null || dest == null)
            {
                throw new ArgumentException();
            }

            if (!vertexIndices.ContainsKey(source) || !vertexIndices.ContainsKey(dest))
            {
                throw new Exception("Source or destination vertex does'nt exist.");
            }

            var sourceIndex = vertexIndices[source];
            var destIndex = vertexIndices[dest];

            if (!matrix[sourceIndex, destIndex].Equals(default(W)))
            {
                return true;
            }

            return false;
        }

        private void doubleMatrixSize()
        {
            var newMatrix = new W[maxSize * 2, maxSize * 2];

            var newVertexIndices = new Dictionary<T, int>();
            var newReverseIndices = new Dictionary<int, T>();

            int k = 0;
            foreach (var vertex in vertexIndices)
            {
                newVertexIndices.Add(vertex.Key, k);
                newReverseIndices.Add(k, vertex.Key);
                k++;
            }

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (!matrix[i, j].Equals(default(W)) 
                        && reverseVertexIndices.ContainsKey(i)
                        && reverseVertexIndices.ContainsKey(j))
                    {
                        var newI = newVertexIndices[reverseVertexIndices[i]];
                        var newJ = newVertexIndices[reverseVertexIndices[j]];

                        newMatrix[newI, newJ] = matrix[i, j];
                    }
                }
            }

            matrix = newMatrix;
            vertexIndices = newVertexIndices;
            reverseVertexIndices = newReverseIndices;
            maxSize *= 2;
        }

        private void halfMatrixSize()
        {
            var newMatrix = new W[maxSize / 2, maxSize / 2];

            var newVertexIndices = new Dictionary<T, int>();
            var newReverseIndices = new Dictionary<int, T>();

            int k = 0;
            foreach (var vertex in vertexIndices)
            {
                newVertexIndices.Add(vertex.Key, k);
                newReverseIndices.Add(k, vertex.Key);
                k++;
            }

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (!matrix[i, j].Equals(default(W)) 
                        && reverseVertexIndices.ContainsKey(i)
                        && reverseVertexIndices.ContainsKey(j))
                    {
                        var newI = newVertexIndices[reverseVertexIndices[i]];
                        var newJ = newVertexIndices[reverseVertexIndices[j]];

                        newMatrix[newI, newJ] = matrix[i, j];
                    }
                }
            }

            matrix = newMatrix;
            vertexIndices = newVertexIndices;
            reverseVertexIndices = newReverseIndices;
            maxSize /= 2;
        }
    }
}
