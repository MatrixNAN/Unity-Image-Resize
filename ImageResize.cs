using UnityEngine;
using System.Collections;

public class ImageResize : MonoBehaviour 
{
	public 	Texture2D 	originalTexture;
	public 	Texture2D 	sizedLargeTexture;
	public 	Texture2D 	sizedMediumTexture;
	public 	Texture2D 	thumbnailTexture;
	public 	int 		targetImageLargeWidth 		= 900;
	public 	int 		targetImageLargeHeight 		= 600;
	public 	int 		targetImageMediumWidth 		= 400;
	public 	int 		targetImageMediumHeight 	= 300;
	public 	int 		targetThumbnailWidth 		= 200;
	public 	int 		targetThumbnailHeight 		= 134;
	private int			determinedWidth				= 0;
	private int 		determinedHeight			= 0;

	public delegate void OnCallbackType ();
	public OnCallbackType OnCallback;

	public void DetermineSize(int targetWidth, int targetHeight)
	{
		float 	ratioOriginal 	= (float) originalTexture.width / (float) originalTexture.height;	
		float 	ratioTarget 	= targetWidth / targetHeight;
		int 	width 			= 0;
		int	 	height 			= 0;
		
		if ( targetHeight > originalTexture.height )
		{
			height = originalTexture.height;
			
			if ( targetWidth > originalTexture.width )
			{
				width = originalTexture.width;
			}
			else
			{
				width = (int) (targetHeight * ratioOriginal);	
			}
		}
		else
		{
			height = targetHeight;
			
			if ( targetWidth > originalTexture.width )
			{
				width = originalTexture.width;
			}
			else
			{
				width = (int) (targetHeight * ratioOriginal);	
			}
		}
		
		/*
		if ( ratioOriginal > ratioTarget )
		{
			Vector2D tempSize = new Vector2D();	
		}
		else if ( ratioOriginal < ratioTarget )
		{
			
		}
		*/
		Debug.Log ("Determined Width: " + width + " Original Texture Width: " + originalTexture.width + " Target Width: " + targetWidth);
		
		determinedWidth  = width;
		determinedHeight = height;
	}
	
	public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight) 
	{
		Debug.Log("Target: " + targetWidth + "x" + targetHeight);
       	Texture2D result = new Texture2D( targetWidth, targetHeight, source.format, true );
       	Color[] rpixels = result.GetPixels(0);
       	float incX = ( (float) 1 / source.width ) * ( (float) source.width / targetWidth );
       	float incY = ( (float) 1 / source.height) * ( (float) source.height / targetHeight );
    
		for( int px=0; px < rpixels.Length; px++ ) 
		{
        	rpixels[px] = source.GetPixelBilinear( incX * ( (float) px % targetWidth ),
                          incY * ( (float) Mathf.Floor( px / targetWidth ) ) );
       	}
       	result.SetPixels( rpixels, 0 );
       	result.Apply();
       	return result;
	}
	
	public byte[] GenerateImage( int targetWidth, int targetHeight )
	{
		DetermineSize( targetWidth, targetHeight );
		Texture2D temp = ScaleTexture ( originalTexture, determinedWidth, determinedHeight );
	    byte[] png = temp.EncodeToPNG();
	    Destroy( temp );
		
		return png;
	}
	
	public void Resize()
	{
		sizedLargeTexture = new Texture2D( determinedWidth, determinedHeight );
	    sizedLargeTexture.LoadImage( GenerateImage( targetImageLargeWidth, targetImageLargeHeight ) );

		sizedMediumTexture = new Texture2D( determinedWidth, determinedHeight );
		sizedMediumTexture.LoadImage( GenerateImage( targetImageMediumWidth, targetImageMediumHeight ) );

		thumbnailTexture = new Texture2D( determinedWidth, determinedHeight );
	    thumbnailTexture.LoadImage( GenerateImage( targetThumbnailWidth, targetThumbnailHeight ) );

		if (OnCallback != null) 
			OnCallback();
		else
		{
			Debug.LogWarning ("Callback not set on Image Resize.");
		}
	}
	
	public void Start()
	{
		// Resize();
	}
}
