// MP Scan
// Given a list (lst) of length n
// Output its prefix sum = {lst[0], lst[0] + lst[1], lst[0] + lst[1] + ... + lst[n-1]}

#include    <wb.h>

#define BLOCK_SIZE 512 //@@ You can change this

#define wbCheck(stmt) do {                                                    \
        cudaError_t err = stmt;                                               \
        if (err != cudaSuccess) {                                             \
            wbLog(ERROR, "Failed to run stmt ", #stmt);                       \
            wbLog(ERROR, "Got CUDA error ...  ", cudaGetErrorString(err));    \
            return -1;                                                        \
        }                                                                     \
    } while(0)
    
__global__ void scan(float * input, 
					 float * output, 
					 float * xy,
					 int len) 
{

  	__shared__ float temp[BLOCK_SIZE];
	
	int x = threadIdx.x + blockDim.x * blockIdx.x;
	temp[threadIdx.x] = (x < len) ? input[x] : 0;
	__syncthreads();
	
	for (int stride = 1; stride < BLOCK_SIZE; stride = stride * 2)                    
    {   
		int ai = (threadIdx.x + 1) * stride * 2 - 1;  
		if (ai < BLOCK_SIZE)
			temp[ai] += temp[ai - stride];
		__syncthreads();  
	}
	for (int stride = BLOCK_SIZE / 2; stride > 0; stride = stride / 2) 
	{  
		int ai = (threadIdx.x + 1) * stride * 2 - 1;  
		if (ai + stride < BLOCK_SIZE)
			temp[ai + stride] += temp[ai];    
		__syncthreads(); 
	}  
	__syncthreads();
	if (blockIdx.x * BLOCK_SIZE + threadIdx.x < len)
	{
		output[blockIdx.x * BLOCK_SIZE + threadIdx.x] = temp[threadIdx.x];
	}
	
	xy[blockIdx.x + 1]  = temp[BLOCK_SIZE - 1];
	xy[0] = 0;
	__syncthreads();
}

__global__ void total(float * output,  float* xy, int len) {
	if (blockIdx.x * BLOCK_SIZE + threadIdx.x < len)
		output[(blockIdx.x) * BLOCK_SIZE + threadIdx.x] += xy[blockIdx.x];
  
}

int main(int argc, char ** argv) {
    wbArg_t args;
    float * hostInput; // The input 1D list
    float * hostOutput; // The output list
    float * deviceInput;
    float * deviceOutput;
	float * device;
  	float * deviceO;
  	float * deviceOp;
    int numElements; // number of elements in the list

    args = wbArg_read(argc, argv);

    wbTime_start(Generic, "Importing data and creating memory on host");
    hostInput = (float *) wbImport(wbArg_getInputFile(args, 0), &numElements);
    hostOutput = (float*) malloc(numElements * sizeof(float));
    wbTime_stop(Generic, "Importing data and creating memory on host");

    wbLog(TRACE, "The number of input elements in the input is ", numElements);

    wbTime_start(GPU, "Allocating GPU memory.");
    wbCheck(cudaMalloc((void**)&deviceInput, numElements*sizeof(float)));
    wbCheck(cudaMalloc((void**)&deviceOutput, numElements*sizeof(float)));
	wbCheck(cudaMalloc((void**)&device, ((numElements - 1) / BLOCK_SIZE + 2)*sizeof(float)));
    wbCheck(cudaMalloc((void**)&deviceO, ((numElements - 1) / BLOCK_SIZE + 2)*sizeof(float)));
	wbCheck(cudaMalloc((void**)&deviceOp, ((numElements - 1) / BLOCK_SIZE + 2)*sizeof(float)));
    wbTime_stop(GPU, "Allocating GPU memory.");

    wbTime_start(GPU, "Clearing output memory.");
    wbCheck(cudaMemset(deviceOutput, 0, numElements*sizeof(float)));
	wbCheck(cudaMemset(device, 0, ((numElements-1) / BLOCK_SIZE + 2)*sizeof(float)));
	wbCheck(cudaMemset(deviceO, 0, ((numElements-1) / BLOCK_SIZE + 2)*sizeof(float)));
	wbCheck(cudaMemset(deviceOp, 0, ((numElements-1) / BLOCK_SIZE + 2)*sizeof(float)));
    wbTime_stop(GPU, "Clearing output memory.");

    wbTime_start(GPU, "Copying input memory to the GPU.");
    wbCheck(cudaMemcpy(deviceInput, hostInput, numElements*sizeof(float), cudaMemcpyHostToDevice));
    wbTime_stop(GPU, "Copying input memory to the GPU.");

    //@@ Initialize the grid and block dimensions here
	dim3 DimGrid((numElements - 1)/BLOCK_SIZE + 1, 1, 1);
	dim3 DimGrid2((((numElements - 1)/BLOCK_SIZE+1)/BLOCK_SIZE) + 1, 1, 1);
	dim3 DimBlock(BLOCK_SIZE, 1, 1);

    wbTime_start(Compute, "Performing CUDA computation");
    //@@ Modify this to complete the functionality of the scan
    //@@ on the deivce
	scan<<<DimGrid, DimBlock>>>(deviceInput, deviceOutput, device, numElements);
	cudaDeviceSynchronize();
  	scan<<<DimGrid2, DimBlock>>>(device, deviceO, deviceOp, ((numElements-1) / BLOCK_SIZE + 2));
    cudaDeviceSynchronize();
	total<<<DimGrid, DimBlock>>>(deviceOutput, deviceO, numElements);
	cudaDeviceSynchronize();
    wbTime_stop(Compute, "Performing CUDA computation");

    wbTime_start(Copy, "Copying output memory to the CPU");
    wbCheck(cudaMemcpy(hostOutput, deviceOutput, numElements*sizeof(float), cudaMemcpyDeviceToHost));
    wbTime_stop(Copy, "Copying output memory to the CPU");

    wbTime_start(GPU, "Freeing GPU Memory");
    cudaFree(deviceInput);
    cudaFree(deviceOutput);
	cudaFree(device);
    cudaFree(deviceO);
    cudaFree(deviceOp);
    wbTime_stop(GPU, "Freeing GPU Memory");

    wbSolution(args, hostOutput, numElements);

    free(hostInput);
    free(hostOutput);

    return 0;
}

