﻿// Begin sample
open FSCL.Compiler.FunctionPreprocessing
open FSCL.Compiler.FunctionCodegen
open FSCL.Compiler.FunctionTransformation
open FSCL.Compiler.ModuleParsing
open FSCL.Compiler.ModulePreprocessing
open FSCL.Compiler.ModuleCodegen
open FSCL.Compiler.Types
open FSCL.Compiler
open FSCL.Compiler.Configuration
open System.IO
open System

[<EntryPoint>]
let main argv = 
    // ***************************************************************************************************************
    //
    // 01: File-based compiler configuration (storage)
    //
    // ***************************************************************************************************************    
    printf("01) Test file-based compiler configuration (storage)\n")
    if Directory.Exists(Compiler.DefaultConfigurationRoot) then
        printf "    Compiler configuration root exists: %s\n" (Compiler.DefaultConfigurationRoot)
    else
        printf "    Compiler configuration root doesn't exist: %s %s\n" (Compiler.DefaultConfigurationRoot)
               "\n    Please create it, copy some components into it and restart the sample\n"
    let compiler = Compiler()
            
    // ***************************************************************************************************************
    //
    // 01: File-based compiler configuration (configuration file implicit)
    //
    // ***************************************************************************************************************    
    printf("02) Test file-based compiler configuration (configuration file)\n")
    printf "    Using configuration file: ImplicitConfiguration.xml"
    let compiler2 = Compiler("ImplicitConfiguration.xml")

    // ***************************************************************************************************************
    //
    // 03: Object-based compiler configuration (implicit file sources)
    //
    // ***************************************************************************************************************
    printf("03) Test object-based compiler configuration (implicit file sources)\n")
    // Copy the dll into the components root
    if not (File.Exists(Path.Combine(Compiler.DefaultConfigurationComponentsRoot,
                                     "FSCL.Compiler.NativeComponents.dll"))) then
        File.Copy("FSCL.Compiler.NativeComponents.dll", Path.Combine(Compiler.DefaultConfigurationComponentsRoot,
                                                                     "FSCL.Compiler.NativeComponents.dll"))
    let configuration = PipelineConfiguration(false, // false = Do not load core sources (explicitely listed as second parameter)
                                              [|
                                                SourceConfiguration(
                                                    FileSource(Path.Combine(
                                                                Compiler.DefaultConfigurationComponentsRoot,
                                                                "FSCL.Compiler.NativeComponents.dll")))
                                              |])
    let compiler3 = Compiler(configuration)
                                              
    // ***************************************************************************************************************
    //
    // 04: Object-based compiler configuration (implicit assembly sources)
    //
    // ***************************************************************************************************************
    printf("04) Test object-based compiler configuration (implicit assembly sources)\n")
    let configuration = PipelineConfiguration(false, // false = Do not load core sources (explicitely listed as second parameter)
                                              [|
                                                SourceConfiguration(
                                                    AssemblySource(typeof<FunctionPreprocessingStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<FunctionTransformationStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<FunctionCodegenStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<ModuleParsingStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<ModulePreprocessingStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<ModuleCodegenStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<DefaultTypeHandler>.Assembly))
                                              |])
    let compiler4 = Compiler(configuration)
                                                                                      
    // ***************************************************************************************************************
    //
    // 05: Object-based compiler configuration (implicit assembly sources except one source, explicit)
    //     All the sources could be made explicit (much verbose for all the core components)
    //
    // ***************************************************************************************************************
    printf("05) Test object-based compiler configuration (explicit assembly source)\n")
    let configuration = PipelineConfiguration(false, // false = Do not load core sources (explicitely listed as second parameter)
                                              [|
                                                SourceConfiguration(                                                                // Explicit source
                                                    AssemblySource(typeof<FunctionPreprocessingStep>.Assembly),                     // The assembly
                                                    [||],                                                                             // No type handlers
                                                    [| StepConfiguration("FSCL_FUNCTION_PREPROCESSING_STEP",                         // Explicitely define steps
                                                                        typeof<FunctionPreprocessingStep>,
                                                                        [| "FSCL_MODULE_PREPROCESSING_STEP"; "FSCL_MODULE_PARSING_STEP" |])|],
                                                    [| StepProcessorConfiguration("FSCL_DATP_PREPROCESSING_PROCESSOR",     // Explicitely define processors
                                                                                 "FSCL_FUNCTION_PREPROCESSING_STEP",
                                                                                 typeof<DynamicArrayToParameterProcessor>);
                                                      StepProcessorConfiguration("FSCL_RTAR_PREPROCESSING_PROCESSOR", 
                                                                                 "FSCL_FUNCTION_PREPROCESSING_STEP",
                                                                                 typeof<RefTypeToArrayReplacingProcessor>,
                                                                                 [|"FSCL_DATP_PREPROCESSING_PROCESSOR"|])|]);
                                                SourceConfiguration(
                                                    AssemblySource(typeof<FunctionTransformationStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<FunctionCodegenStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<ModuleParsingStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<ModulePreprocessingStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<ModuleCodegenStep>.Assembly));
                                                SourceConfiguration(
                                                    AssemblySource(typeof<DefaultTypeHandler>.Assembly))
                                              |])
                                              
    let compiler5 = Compiler(configuration)
       
    // ***************************************************************************************************************
    //
    // 06: Instance-based compiler configuration (the dependency is implicit in the components order, useful for rapid debugging)
    //
    // ***************************************************************************************************************
   (* printf("06) Test instance-based compiler configuration\n")
    let configuration = PipelineConfiguration(true, [
                                                new ModuleParsingStep(
                                                    null,
                                                    [
                                                        new KernelReferenceParser();
                                                        new KernelMethodInfoParser()
                                                    ])
                                              ],
                                              [])
    let compiler3 = Compiler(configuration)
     *)             
    Console.WriteLine("Press enter to exit")
    Console.Read() |> ignore
    0