//MP3 Tiled Matrix-Matrix Multiplication

#include <wb.h>

#define wbCheck(stmt)                                                          \
  do {                                                                         \
    cudaError_t err = stmt;                                                    \
    if (err != cudaSuccess) {                                                  \
      wbLog(ERROR, "Failed to run stmt ", #stmt);                              \
      wbLog(ERROR, "Got CUDA error ...  ", cudaGetErrorString(err));           \
      return -1;                                                               \
    }                                                                          \
  } while (0)

// Compute C = A * B
__global__ void matrixMultiplyShared(float *A, float *B, float *C, 
									 int numARows, int numAColumns, 
									 int numBRows, int numBColumns, 
									 int numCRows, int numCColumns) 
{
  //@@ Insert code to implement matrix multiplication here
  //@@ You have to use shared memory for this MP
	const int TILE_WIDTH = 16;
  	__shared__ float _A[TILE_WIDTH][TILE_WIDTH];
	__shared__ float _B[TILE_WIDTH][TILE_WIDTH];
	int col = blockDim.x * blockIdx.x + threadIdx.x;
    int row = blockDim.y * blockIdx.y + threadIdx.y;
	int bx = blockIdx.x; 
	int by = blockIdx.y;
	int tx = threadIdx.x; 
	int ty = threadIdx.y;
	
	int rowA = row;
	int colA = tx;
	int rowB = ty;
	int colB = col;
	
	float sum = 0.0;
	
	for (int i = 0; i < ( (numAColumns -1) / TILE_WIDTH ) + 1; i++) 
	{
		if (rowA < numARows && colA < numAColumns) 
		{
			_A[ty][tx] = A[rowA * numAColumns + colA];
		} 
		else 
		{
			_A[ty][tx] = 0.0;
		}
		
		if (rowB < numBRows && colB < numBColumns) 
		{
			_B[ty][tx] = B[rowB * numBColumns + colB];
		} 
		else 
		{
			_B[ty][tx] = 0.0;
		}
		__syncthreads();
		
		for (int j = 0; j < TILE_WIDTH; j++) 
		{
			sum += _A[ty][j] * _B[j][tx];
		} 
		colA +=TILE_WIDTH;
		rowB +=TILE_WIDTH;
		__syncthreads();
	}
	
    if (row < numCRows && col < numCColumns) 
	{
        C[row * numCColumns + col] = sum;
		
	}
}

int main(int argc, char **argv) {
  wbArg_t args;
  float *hostA; // The A matrix
  float *hostB; // The B matrix
  float *hostC; // The output C matrix
  float *deviceA;
  float *deviceB;
  float *deviceC;
  int numARows;    // number of rows in the matrix A
  int numAColumns; // number of columns in the matrix A
  int numBRows;    // number of rows in the matrix B
  int numBColumns; // number of columns in the matrix B
  int numCRows;    // number of rows in the matrix C (you have to set this)
  int numCColumns; // number of columns in the matrix C (you have to set this)

  args = wbArg_read(argc, argv);

  wbTime_start(Generic, "Importing data and creating memory on host");
  hostA =
      ( float * )wbImport(wbArg_getInputFile(args, 0), &numARows, &numAColumns);
  hostB =
      ( float * )wbImport(wbArg_getInputFile(args, 1), &numBRows, &numBColumns);
  //@@ Set numCRows and numCColumns
  numCRows = numARows;
  numCColumns = numBColumns;
  //@@ Allocate the hostC matrix
  hostC = 
	  ( float * ) malloc(numCRows * numCColumns * sizeof(float));
  wbTime_stop(Generic, "Importing data and creating memory on host");

  wbLog(TRACE, "The dimensions of A are ", numARows, " x ", numAColumns);
  wbLog(TRACE, "The dimensions of B are ", numBRows, " x ", numBColumns);

  wbTime_start(GPU, "Allocating GPU memory.");
  //@@ Allocate GPU memory here
  wbCheck( cudaMalloc( (void **)&deviceA, 
					   sizeof(float) * numARows * numAColumns ) );
  wbCheck( cudaMalloc( (void **)&deviceB, 
					   sizeof(float) * numBRows * numBColumns ) );
  wbCheck( cudaMalloc( (void **)&deviceC, 
					   sizeof(float) * numCRows * numCColumns ) );

  wbTime_stop(GPU, "Allocating GPU memory.");

  wbTime_start(GPU, "Copying input memory to the GPU.");
  //@@ Copy memory to the GPU here
  wbCheck( cudaMemcpy(deviceA, hostA, 
					    sizeof(float) * numARows * numAColumns,
                        cudaMemcpyHostToDevice) );
  wbCheck( cudaMemcpy(deviceB, hostB, 
					    sizeof(float) * numBRows * numBColumns,
                        cudaMemcpyHostToDevice) );

  wbTime_stop(GPU, "Copying input memory to the GPU.");

  //@@ Initialize the grid and block dimensions here
  dim3 dimGrid( ( (numCColumns - 1) / 16.0 ) + 1, 

			    ( (numCRows    - 1) / 16.0 ) + 1, 1 );
  dim3 dimBlock(16, 16, 1);

  wbTime_start(Compute, "Performing CUDA computation");
  //@@ Launch the GPU Kernel here
  matrixMultiplyShared<<<dimGrid,dimBlock>>>(deviceA, deviceB, deviceC, 
									   	     numARows, numAColumns,
                                       		 numBRows, numBColumns, 
									   		 numCRows, numCColumns);

  cudaDeviceSynchronize();
  wbTime_stop(Compute, "Performing CUDA computation");

  wbTime_start(Copy, "Copying output memory to the CPU");
  //@@ Copy the GPU memory back to the CPU here
  wbCheck( cudaMemcpy(hostC, deviceC,
                        sizeof(float) * numCRows * numCColumns,
                        cudaMemcpyDeviceToHost) );

  wbTime_stop(Copy, "Copying output memory to the CPU");

  wbTime_start(GPU, "Freeing GPU Memory");
  //@@ Free the GPU memory here
  wbCheck( cudaFree(deviceA) );
  wbCheck( cudaFree(deviceB) );
  wbCheck( cudaFree(deviceC) );

  wbTime_stop(GPU, "Freeing GPU Memory");

  wbSolution(args, hostC, numCRows, numCColumns);

  free(hostA);
  free(hostB);
  free(hostC);

  return 0;
}