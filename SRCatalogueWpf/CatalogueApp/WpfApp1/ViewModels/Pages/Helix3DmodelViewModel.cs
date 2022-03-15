using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Wpf.SharpDX.Assimp;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using MaterialDesignThemes.Wpf;

namespace WpfApp1.ViewModels.Pages
{
    class Helix3DmodelViewModel : IPageViewModel, INotifyPropertyChanged
    {
        public PackIconKind Icon
        {
            get { return MaterialDesignThemes.Wpf.PackIconKind.CubeOutline; }
        }

        public void Dispose() { }
        public void Load() { }

        public string IdentificationName
        {
            get { return "HelixModel"; }
        }

        private Visibility _visibility;

        public Visibility TreeVisibility
        {
            get
            {
                return _visibility;
            }
        }

        public string model_path;

        public Helix3DmodelViewModel(DataRow ModelDataRow, Database database)
        {
            model_path = database.soursePath + database.CatalogueModels + ModelDataRow[1].ToString();
            
           // this.OpenFileCommand = new DelegateCommand((obj) => { this.OpenFile(); });
            _EffectsManager = new DefaultEffectsManager();

            _visibility = database.IsAdmin;
            
                      
            _Camera = new PerspectiveCamera()
            {

                Position = new System.Windows.Media.Media3D.Point3D(-10, 10, -10),
                LookDirection= new System.Windows.Media.Media3D.Vector3D(10, -10, 10),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = double.PositiveInfinity,
                NearPlaneDistance = 0.1f,  
                FieldOfView=45
                
            };

           // EnvironmentMap = LoadFileToMemory("tekstura.dds");

        }

        //public static MemoryStream LoadFileToMemory(string filePath)
        //{
        //    using (var file = new FileStream(filePath, FileMode.Open))
        //    {
        //        var memory = new MemoryStream();
        //        file.CopyTo(memory);
        //        return memory;
        //    }
        //}

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                showWireframe = value;
                ShowWireframeFunct(value);

            }
            get
            {
                return showWireframe;
            }
        }

        private bool renderFlat=true;
        public bool RenderFlat
        {
            set
            {
                renderFlat = value;

                RenderFlatFunct(value);

            }
            get
            {
                return renderFlat;
            }
        }

        private bool renderEnvironmentMap = false;
        public bool RenderEnvironmentMap
        {
            set
            {
                if (scene != null && scene.Root != null)
                {
                    renderEnvironmentMap = value;

                    foreach (var node in scene.Root.Traverse())
                    {
                        if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
                        {
                            material.RenderEnvironmentMap = value;
                        }
                    }
                }
            }
            get => renderEnvironmentMap;
        }

       
        private bool isLoading = false;
        public bool IsLoading
        {
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
            get
            {
                return isLoading;
            }
        }

        private bool enableAnimation = false;
        public bool EnableAnimation
        {
            set
            {
                enableAnimation = value;

                if (value)
                {
                    StartAnimation();
                }
                else
                {
                    StopAnimation();
                }

            }
            get { return enableAnimation; }
        }

        public ObservableCollection<Animation> Animations { get; } = new ObservableCollection<Animation>();

        public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();

        private Animation selectedAnimation = null;
        public Animation SelectedAnimation
        {
            set
            {
                selectedAnimation = value;
                

                StopAnimation();
                if (value != null)
                {
                    animationUpdater = new NodeAnimationUpdater(value);
                }
                else
                {
                    animationUpdater = null;
                }
                if (enableAnimation)
                {
                    StartAnimation();
                }

            }
            get
            {
                return selectedAnimation;
            }
        }

        public TextureModel EnvironmentMap { get; }

        private SynchronizationContext context = SynchronizationContext.Current;
        private HelixToolkitScene scene;
        private NodeAnimationUpdater animationUpdater;
        private List<BoneSkinMeshNode> boneSkinNodes = new List<BoneSkinMeshNode>();
        private List<BoneSkinMeshNode> skeletonNodes = new List<BoneSkinMeshNode>();
        private CompositionTargetEx compositeHelper = new CompositionTargetEx(); //для анимации

        private PerspectiveCamera _Camera;       
        public PerspectiveCamera Camera
        {
            get { return _Camera; }
        }
        
        private EffectsManager _EffectsManager;
        public EffectsManager EffectsManager
        {
            get { return _EffectsManager; }
        }

       
        public void OpenFile()
        {
            if (isLoading)
            {
                return;
            }
           //  string path = OpenFileDialog(OpenFileFilter);
           string path = model_path;
            if (path == null)
            {
                return;
            }
            StopAnimation();

            IsLoading = true;

            Task.Run(() =>
            {                   
                var loader = new Importer();
                return loader.Load(path);

            }).ContinueWith((result) =>
            {
               

                if (result.IsCompleted)
                {
                    scene = result.Result;
                    
                    Animations.Clear();
                    GroupModel.Clear();                    
                    if (scene != null)
                    {
                        if (scene.Root != null)
                        {
                            foreach (var node in scene.Root.Traverse())
                            {
                                if (node is MaterialGeometryNode m)
                                {
                                    if (m.Material is PBRMaterialCore pbr)
                                    {
                                        pbr.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                    else if (m.Material is PhongMaterialCore phong)
                                    {
                                        phong.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                }
                            }
                        }

                        GroupModel.AddNode(scene.Root);

                        if (scene.HasAnimation)
                        {
                            foreach (var ani in scene.Animations)
                            {
                                Animations.Add(ani);
                            }
                            SelectedAnimation = Animations[0];
                        }

                        foreach (var n in scene.Root.Traverse())
                        {
                            n.Tag = new AttachedNodeViewModel(n);
                            
                        }

                        RenderFlat = true;                      
                       

                    }
                }
                else if (result.IsFaulted && result.Exception != null)
                {
                    System.Windows.MessageBox.Show(result.Exception.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith((result)=> {

                if (result.IsCompleted)
                {
                    IsLoading = false;
                }
            });
            
        }

        public void StartAnimation()
        {
            compositeHelper.Rendering += CompositeHelper_Rendering;
        }

        public void StopAnimation()
        {
            compositeHelper.Rendering -= CompositeHelper_Rendering;
        }

        private void CompositeHelper_Rendering(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            if (animationUpdater != null)
            {
                animationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
            }
        }

       

        private void ShowWireframeFunct(bool show)
        {
            foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
            {
                return node.IsRenderable;
            }))
            {
                if (node is MeshNode m)
                {
                    m.RenderWireframe = show;
                }
            }
        }

        private void RenderFlatFunct(bool show)
        {
            foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
            {
                return node.IsRenderable;
            }))
            {
                if (node is MeshNode m)
                {
                    if (m.Material is PhongMaterialCore phong)
                    {
                        phong.EnableFlatShading = show;
                    }
                    else if (m.Material is PBRMaterialCore pbr)
                    {
                        pbr.EnableFlatShading = show;
                    }
                }
            }
        }


        private RelayCommand zoomExt;
        public RelayCommand ZoomExt
        {
            get
            {
                return zoomExt ??
                  (zoomExt = new RelayCommand(obj =>
                  {
                      (obj as Viewport3DX).ZoomExtents();
                      
                  },
                (obj) => IsLoading==false)); // 
            }
        }

        private RelayCommand loadModel;
        public RelayCommand LoadModel
        {
            get
            {
                return loadModel ??
                  (loadModel = new RelayCommand(obj =>
                  {
                      OpenFile();

                  },
                (obj) => IsLoading == false)); // 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
