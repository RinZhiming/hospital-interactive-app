using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;
using MonsterLove.StateMachine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnityExample.DnnModel;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum FaceScan
{
    Scan,
    Complete,
}

public class FaceScanner : MonoBehaviour
{
    [SerializeField] private GameObject checkMark;
    [SerializeField] private string sceneInformation;
    [SerializeField] private GameObject[] hideUi;
    [SerializeField] private RawImage image;
    [SerializeField] private WebCamTextureToMatHelper webCamTextureToMatHelper;
    [SerializeField] private float torelance;
    [SerializeField] private float delayDetectTime = 3;

    private static readonly string FACE_DETECTION_MODEL_FILENAME = "OpenCVForUnity/dnn/face_detection_yunet_2023mar.onnx";
    private string faceDetectionModelFilepath;
    private StateMachine<FaceScan> facescanFsm;
    private DatabaseReference dataRef;
    private FirebaseAuth auth;
    private FirebaseStorage storage;
    private YuNetV2FaceDetector faceDetector;
    private Texture2D texture;
    private bool canScan, isDetect;
    private int inputSizeW = 320;
    private int inputSizeH = 320;
    private float scoreThreshold = 0.7f;
    private float nmsThreshold = 0.3f;
    private int topK = 5000;
    private Mat bgrMat;
    private Vector2 facePosition;
    //470 200
    private const float centerX = (450f - 3f) / 2f;
    private const float centerY = (853f + 7f) / 2f;
    private float time;
    [SerializeField] private float angels;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        facescanFsm = StateMachine<FaceScan>.Initialize(this);
        canScan = false;
        isDetect = false;
    }

    private IEnumerator Start()
    {
        checkMark.SetActive(false);

        auth = FirebaseAuth.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
        
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        faceDetectionModelFilepath = Utils.getFilePath(FACE_DETECTION_MODEL_FILENAME);
        faceDetector = new YuNetV2FaceDetector(faceDetectionModelFilepath, "", new Size(inputSizeW, inputSizeH), scoreThreshold, nmsThreshold, topK);
        webCamTextureToMatHelper.avoidAndroidFrontCameraLowLightIssue = true;
        webCamTextureToMatHelper.requestedIsFrontFacing = true;

        
        webCamTextureToMatHelper.Initialize();
        
        yield return new WaitUntil(() => webCamTextureToMatHelper.IsInitialized());
        
        webCamTextureToMatHelper.Play();
    }

    public void OnWebcamInit()
    {
        var webCamTextureMat = webCamTextureToMatHelper.GetMat();
        // webCamTextureToMatHelper.requestedWidth = 500;
        // webCamTextureToMatHelper.requestedHeight = 500;
        if (webCamTextureMat != null)
        {
            
            var width = webCamTextureMat.width();
            var height = webCamTextureMat.height();
            
            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
            
            Utils.matToTexture2D(webCamTextureMat, texture);

            image.texture = texture;
            image.color = Color.white;
            
            float screenRatio = (float)webCamTextureMat.cols() / (float)webCamTextureMat.rows();
            
            image.rectTransform.sizeDelta = new Vector2(width * screenRatio, height);
            
            // Debug.Log("image display size : " + image.rectTransform.sizeDelta);
            //
            // Debug.Log("texture width : " + texture.width);
            // Debug.Log("texture height : " + texture.height);
            //
            // Debug.Log("webCamTextureMat width : " + webCamTextureMat.cols());
            // Debug.Log("webCamTextureMat height : " + webCamTextureMat.rows());

            bgrMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);

            facescanFsm.ChangeState(FaceScan.Scan);
        }
    }

    public void OnWebcamDispose()
    {
        bgrMat?.Dispose();

        if (texture)
        {
            Destroy(texture);
            texture = null;
        }
    }

    public void OnWebcamError(WebCamTextureToMatHelper.ErrorCode errorCode)
    {
        Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
    }

    private void OnDestroy()
    {
        webCamTextureToMatHelper?.Dispose();

        faceDetector?.dispose();
    }

    private void Update()
    {
        if (canScan)
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                var rgbamat = webCamTextureToMatHelper.GetMat();

                if (faceDetector != null)
                {
                    Imgproc.cvtColor(rgbamat, bgrMat, Imgproc.COLOR_RGBA2BGR);

                    var faces = faceDetector.infer(bgrMat);

                    Imgproc.cvtColor(bgrMat, rgbamat, Imgproc.COLOR_BGR2RGBA);

                    //faceDetector.visualize(rgbamat, faces, false, true);
                    

                    foreach (var face in faceDetector.getData(faces))
                    {
                        facePosition = face.xy;
                    }

                }
                
                var rotImage = Imgproc.getRotationMatrix2D(new Point(rgbamat.cols() / 2,
                    rgbamat.rows() / 2), angels, 0.625f);
                Imgproc.warpAffine(rgbamat, rgbamat, rotImage, rgbamat.size());
                Imgproc.warpAffine(bgrMat, bgrMat, rotImage, bgrMat.size());

                Utils.matToTexture2D(rgbamat, texture);
            }
        }
    }

    public void Scan_Enter()
    {
        canScan = true;
    }
    public void Scan_Update()
    {
        if (canScan && PositionAdjuster(facePosition).Item2)
        {
            checkMark.SetActive(true);
            
            time += Time.deltaTime;

            if (time >= delayDetectTime)
            {
                facescanFsm.ChangeState(FaceScan.Complete);
                time = 0;
            }
        }
        else
        {
            checkMark.SetActive(false);
            time = 0;
        }
    }

    public void Complete_Enter()
    {
        StartCoroutine(SceneShot());
    }

    private IEnumerator SceneShot()
    {
        foreach (var h in hideUi)
        {
            h.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        Texture2D sh = ScreenCapture.CaptureScreenshotAsTexture();

        var screenshotRef = storage.GetReference($"/FaceVerify/{auth.CurrentUser.UserId}.jpg");
        var bytes = sh.EncodeToJPG();
        var uploadTask = screenshotRef.PutBytesAsync(bytes);
        
        foreach (var h in hideUi)
        {
            h.SetActive(true);
        }

        if (uploadTask.Exception != null)
        {
            Debug.Log($"Failed to upload {uploadTask.Exception}");
            yield break;
        }

        Debug.Log($"Complete to upload.");

        UpdateDatabase();
    }


    private void UpdateDatabase()
    {
        dataRef.Child("users").Child(auth.CurrentUser.UserId).Child("userverify").UpdateChildrenAsync(new Dictionary<string, object>() { { "isFaceVerify", true } }).ContinueWithOnMainThread(result =>
        {
            canScan = false;
            checkMark.SetActive(false);
            SceneManager.LoadScene(sceneInformation);
        });
    }
    
    private (Vector2, bool) PositionAdjuster(Vector2 pos)
    {
        var adjustX = Mathf.Max(centerX - torelance, Mathf.Min(pos.x,centerX + torelance));
        var adjustY = Mathf.Max(centerY - torelance, Mathf.Min(pos.y,centerY + torelance));
        
        isDetect = pos.x >= centerX - torelance && pos.x <= centerX + torelance &&
                   pos.y >= centerY - torelance && pos.y <= centerY + torelance;
        
        return (new Vector2(adjustX, adjustY), isDetect);
    }
}