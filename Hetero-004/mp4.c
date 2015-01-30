#include <wb.h>


#define wbCheck(stmt) do {                                                    \
        cudaError_t err = stmt;                                               \
        if (err != cudaSuccess) {                                             \
            wbLog(ERROR, "Failed to run stmt ", #stmt);                       \
            wbLog(ERROR, "Got CUDA error ...  ", cudaGetErrorString(err));    \
            return -1;                                                        \
        }                                                                     \
    } while(0)

#define Mask_width  5
#define Mask_radius (Mask_width/2)
#define TILE_WIDTH 16
#define BLOCK_WIDTH (TILE_WIDTH + Mask_width - 1)
#define CHANNELS 3
 
//@@ INSERT CODE HERE
__global__ void convolutionKernel(float* in, float* out, 
							int height, int width, 
							int channels,
							const float* __restrict__ M)
{
	__shared__ float Ns[BLOCK_WIDTH][BLOCK_WIDTH][CHANNELS];
	//__shared__ float Ns[BLOCK_WIDTH][BLOCK_WIDTH][channels]; //assuming channels == 3
	
	int bx = blockIdx.x;  
	int by = blockIdx.y; 
	int tx = threadIdx.x;  
	int ty = threadIdx.y;
	int rowIndex = by * TILE_WIDTH + ty - Mask_radius;
	int colIndex = bx * TILE_WIDTH + tx - Mask_radius;
	for (int channelIndex = 0; channelIndex < channels; ++channelIndex)
	{
		if (rowIndex >=0 && rowIndex < height &&
		colIndex >=0 && colIndex < width)
		{
			Ns[ty][tx][channelIndex] = 
				in[(rowIndex * width + colIndex) * channels + channelIndex];
		}
		else
		{
			Ns[ty][tx][channelIndex] = 0.0;
		}
	}
	__syncthreads();
	rowIndex = rowIndex + Mask_radius;
	colIndex = colIndex + Mask_radius;
	if (rowIndex >= height || colIndex >= width ||
		 ty >= TILE_WIDTH || tx >= TILE_WIDTH)
		return;
		
	for (int channelIndex = 0; channelIndex < channels; ++ channelIndex)
	{
		float accum = 0.0;
		for (int y = 0; y < Mask_width; y++)
			for (int x = 0; x < Mask_width; x++)
				accum += M[y * Mask_width + x] * Ns[y + ty][x + tx][channelIndex];
			
		out[(rowIndex * width + colIndex) * channels + channelIndex] = accum;
	}
}


int main(int argc, char* argv[]) {
    wbArg_t args;
    int maskRows;
    int maskColumns;
    int imageChannels;
    int imageWidth;
    int imageHeight;
    char * inputImageFile;
    char * inputMaskFile;
    wbImage_t inputImage;
    wbImage_t outputImage;
    float * hostInputImageData;
    float * hostOutputImageData;
    float * hostMaskData;
    float * deviceInputImageData;
    float * deviceOutputImageData;
    float * deviceMaskData;

    args = wbArg_read(argc, argv); /* parse the input arguments */

    inputImageFile = wbArg_getInputFile(args, 0);
    inputMaskFile = wbArg_getInputFile(args, 1);

    inputImage = wbImport(inputImageFile);
    hostMaskData = (float *) wbImport(inputMaskFile, &maskRows, &maskColumns);

    assert(maskRows == 5); /* mask height is fixed to 5 in this mp */
    assert(maskColumns == 5); /* mask width is fixed to 5 in this mp */

    imageWidth = wbImage_getWidth(inputImage);
    imageHeight = wbImage_getHeight(inputImage);
    imageChannels = wbImage_getChannels(inputImage);

    outputImage = wbImage_new(imageWidth, imageHeight, imageChannels);

    hostInputImageData = wbImage_getData(inputImage);
    hostOutputImageData = wbImage_getData(outputImage);

    wbTime_start(GPU, "Doing GPU Computation (memory + compute)");

    wbTime_start(GPU, "Doing GPU memory allocation");
    cudaMalloc((void **) &deviceInputImageData, imageWidth * imageHeight * imageChannels * sizeof(float));
    cudaMalloc((void **) &deviceOutputImageData, imageWidth * imageHeight * imageChannels * sizeof(float));
    cudaMalloc((void **) &deviceMaskData, maskRows * maskColumns * sizeof(float));
    wbTime_stop(GPU, "Doing GPU memory allocation");


    wbTime_start(Copy, "Copying data to the GPU");
    cudaMemcpy(deviceInputImageData,
               hostInputImageData,
               imageWidth * imageHeight * imageChannels * sizeof(float),
               cudaMemcpyHostToDevice);
    cudaMemcpy(deviceMaskData,
               hostMaskData,
               maskRows * maskColumns * sizeof(float),
               cudaMemcpyHostToDevice);
    wbTime_stop(Copy, "Copying data to the GPU");


    wbTime_start(Compute, "Doing the computation on the GPU");
    //@@ INSERT CODE HERE
	dim3 dimBlock(BLOCK_WIDTH, BLOCK_WIDTH);
	dim3 dimGrid(ceil(imageWidth / (float) TILE_WIDTH), 
             	 ceil(imageHeight / (float) TILE_WIDTH));
	convolutionKernel<<<dimGrid, dimBlock>>>(deviceInputImageData, deviceOutputImageData,
											 imageHeight, imageWidth, imageChannels, deviceMaskData);
    wbTime_stop(Compute, "Doing the computation on the GPU");


    wbTime_start(Copy, "Copying data from the GPU");
    cudaMemcpy(hostOutputImageData,
               deviceOutputImageData,
               imageWidth * imageHeight * imageChannels * sizeof(float),
               cudaMemcpyDeviceToHost);
    wbTime_stop(Copy, "Copying data from the GPU");

    wbTime_stop(GPU, "Doing GPU Computation (memory + compute)");

    wbSolution(args, outputImage);

    cudaFree(deviceInputImageData);
    cudaFree(deviceOutputImageData);
    cudaFree(deviceMaskData);

    free(hostMaskData);
    wbImage_delete(outputImage);
    wbImage_delete(inputImage);

    return 0;
}
